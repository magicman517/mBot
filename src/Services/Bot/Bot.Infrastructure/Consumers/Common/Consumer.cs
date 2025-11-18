using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Rest;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Common;

namespace Bot.Infrastructure.Consumers.Common;

public abstract class Consumer<T>(IConnection connection, ILogger logger) : BackgroundService
{
    protected string? ExchangeName { get; set; }
    protected string? QueueName { get; set; }
    protected string? RoutingKey { get; set; }

    private IChannel? _channel;
    
    protected abstract void Configure();
    protected abstract Task HandleAsync(T body, CancellationToken ct = default);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        Configure();
        ValidateConfiguration();
        
        _channel = await connection.CreateChannelAsync(cancellationToken: ct);
        
        await _channel.ExchangeDeclareAsync(
            ExchangeName!,
            ExchangeType.Topic,
            durable: true,
            cancellationToken: ct);
        
        await _channel.QueueDeclareAsync(
            QueueName!,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);
        
        await _channel.QueueBindAsync(
            QueueName!,
            ExchangeName!,
            RoutingKey!,
            cancellationToken: ct);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, args) =>
        {
            if (ct.IsCancellationRequested)
            {
                await _channel.BasicNackAsync(args.DeliveryTag, false, true, ct);
                return;
            }

            try
            {
                var jsonString = Encoding.UTF8.GetString(args.Body.Span);
                var body = JsonSerializer.Deserialize<T>(jsonString);
                if (body is null)
                    throw new Exception($"Failed to deserialize {typeof(T).Name}");
                
                await HandleAsync(body, ct);
                await _channel.BasicAckAsync(args.DeliveryTag, false, ct);
            }
            catch (JsonException)
            {
                await _channel.BasicNackAsync(args.DeliveryTag, false, false, ct);
            }
            catch (Exception)
            {
                await _channel.BasicNackAsync(args.DeliveryTag, false, true, ct);
            }
        };
        
        await _channel.BasicConsumeAsync(
            QueueName!,
            autoAck: false,
            consumer: consumer,
            cancellationToken: ct);
        
        await Task.Delay(Timeout.Infinite, ct);
    }
    
    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(ExchangeName)
            || string.IsNullOrWhiteSpace(QueueName)
            || string.IsNullOrWhiteSpace(RoutingKey))
            throw new InvalidOperationException("Invalid consumer configuration");
    }
}
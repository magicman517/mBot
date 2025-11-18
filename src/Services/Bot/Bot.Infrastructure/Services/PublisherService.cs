using System.Text.Json;
using Bot.Core.Interfaces.Services;
using Bot.Infrastructure.ChannelPool;
using RabbitMQ.Client;

namespace Bot.Infrastructure.Services;

public class PublisherService(IAsyncChannelPool asyncChannelPool) : IPublisherService
{
    public async Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken ct = default)
    {
        var channel = await asyncChannelPool.GetChannelAsync(ct);
        try
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(message);
            
            var properties = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: ct);
        }
        finally
        {
            await asyncChannelPool.ReturnChannelAsync(channel, ct);
        }
    }
}
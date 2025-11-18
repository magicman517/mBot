namespace Bot.Core.Interfaces.Services;

public interface IPublisherService
{
    Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken ct = default);
}
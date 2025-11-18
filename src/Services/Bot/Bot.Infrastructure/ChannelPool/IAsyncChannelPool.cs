using RabbitMQ.Client;

namespace Bot.Infrastructure.ChannelPool;

public interface IAsyncChannelPool
{
    Task<IChannel> GetChannelAsync(CancellationToken ct = default);
    Task<bool> ReturnChannelAsync(IChannel channel, CancellationToken ct = default);
}
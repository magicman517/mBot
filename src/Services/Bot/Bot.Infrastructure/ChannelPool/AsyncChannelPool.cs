using System.Collections.Concurrent;
using RabbitMQ.Client;

namespace Bot.Infrastructure.ChannelPool;

public class AsyncChannelPool(IConnection connection) : IAsyncChannelPool, IAsyncDisposable
{
    private readonly ConcurrentBag<IChannel> _pool = [];
    
    private const int PoolCapacity = 50;
    
    public async Task<IChannel> GetChannelAsync(CancellationToken ct = default)
    {
        if (!_pool.TryTake(out var channel))
            return await connection.CreateChannelAsync(cancellationToken: ct);
        
        if (channel.IsOpen)
            return channel;

        await channel.DisposeAsync();

        return await connection.CreateChannelAsync(cancellationToken: ct);
    }

    public async Task<bool> ReturnChannelAsync(IChannel channel, CancellationToken ct = default)
    {
        if (channel.IsOpen && _pool.Count < PoolCapacity)
        {
            _pool.Add(channel);
            return true;
        }

        await channel.DisposeAsync();
        return false;
    }

    public async ValueTask DisposeAsync()
    {
        while (_pool.TryTake(out var channel))
        {
            await channel.DisposeAsync();
        }
    }
}
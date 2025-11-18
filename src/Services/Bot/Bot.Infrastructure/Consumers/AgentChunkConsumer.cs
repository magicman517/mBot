using Bot.Infrastructure.Consumers.Common;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using RabbitMQ.Client;
using Shared.Contracts;

namespace Bot.Infrastructure.Consumers;

public class AgentChunkConsumer(IConnection connection, ILogger<AgentChunkConsumer> logger, GatewayClient gatewayClient)
    : DiscordConsumer<AgentChunkContract>(connection, logger, gatewayClient)
{
    protected override void Configure()
    {
        ExchangeName = "agent.tasks";
        QueueName = "agent.tasks.chunks";
        RoutingKey = "agent.tasks.chunks";
    }

    protected override async Task HandleAsync(AgentChunkContract body, CancellationToken ct = default)
    {
        if (!TryGetContextIds(body.ChatContext, out var guildId, out var channelId))
        {
            logger.LogWarning("Invalid guild or channel ID in chat context: {ChatContext}", body.ChatContext);
            return;
        }

        if (!TryGetChannel(guildId, channelId, out var channel))
        {
            logger.LogWarning("Channel not found: GuildId={GuildId}, ChannelId={ChannelId}", guildId, channelId);
            return;
        }

        await channel.SendMessageAsync(new MessageProperties
        {
            Content = body.Content,
            Flags = MessageFlags.SuppressNotifications
        }, cancellationToken: ct);
    }
}
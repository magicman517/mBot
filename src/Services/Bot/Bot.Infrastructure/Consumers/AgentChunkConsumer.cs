using Bot.Infrastructure.Consumers.Common;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using RabbitMQ.Client;
using Shared.Common;
using Shared.Contracts;

namespace Bot.Infrastructure.Consumers;

public class AgentChunkConsumer(IConnection connection, ILogger<AgentChunkConsumer> logger, GatewayClient gatewayClient)
    : Consumer<AgentChunkContract>(connection)
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

    private bool TryGetContextIds(ChatContext context, out ulong guildId, out ulong channelId)
    {
        channelId = 0;
        return ulong.TryParse(context.GuildId, out guildId) &&
               ulong.TryParse(context.ChannelId, out channelId);
    }

    private bool TryGetChannel(ulong guildId, ulong channelId, out TextChannel channel)
    {
        channel = null!;

        if (!gatewayClient.Cache.Guilds.TryGetValue(guildId, out var guild) ||
            !guild.Channels.TryGetValue(channelId, out var genericChannel) ||
            genericChannel is not TextChannel textChannel) return false;

        channel = textChannel;
        return true;
    }
}
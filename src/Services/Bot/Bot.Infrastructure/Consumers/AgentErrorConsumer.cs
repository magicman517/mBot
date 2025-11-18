using Bot.Infrastructure.Consumers.Common;
using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using RabbitMQ.Client;
using Shared.Contracts;

namespace Bot.Infrastructure.Consumers;

public class AgentErrorConsumer(IConnection connection, ILogger<AgentErrorConsumer> logger, GatewayClient gatewayClient)
    : DiscordConsumer<AgentErrorContract>(connection, logger, gatewayClient)
{
    protected override void Configure()
    {
        ExchangeName = "agent.tasks";
        QueueName = "agent.tasks.errors";
        RoutingKey = "agent.tasks.errors";
    }

    protected override async Task HandleAsync(AgentErrorContract body, CancellationToken ct = default)
    {
        if (!TryGetContextIds(body.ChatContext, out var guildId, out var channelId))
        {
            logger.LogWarning("Invalid guild or channel ID in chat context: {ChatContext}", body.ChatContext);
            return;
        }

        if (!TryGetChannel(guildId, channelId, out var textChannel))
        {
            logger.LogWarning("Channel not found: GuildId={GuildId}, ChannelId={ChannelId}", guildId, channelId);
            return;
        }

        var content = FormatResponse(body);
        var messageProperties = CreateMessageProperties(content);

        var sentAsReply = await TryReplyAsync(textChannel, body.ChatContext.PromptMessageId, messageProperties, ct);
        if (!sentAsReply)
            await textChannel.SendMessageAsync(messageProperties, cancellationToken: ct);
    }

    private static string FormatResponse(AgentErrorContract body)
    {
        var content = body.Message;
        return content.Length > 100
            ? $"{content[..100]}..."
            : content;
    }
}
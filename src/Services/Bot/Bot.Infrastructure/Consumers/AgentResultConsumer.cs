using Bot.Infrastructure.Consumers.Common;
using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using RabbitMQ.Client;
using Shared.Contracts;

namespace Bot.Infrastructure.Consumers;

public class AgentResultConsumer(
    IConnection connection,
    GatewayClient gatewayClient,
    ILogger<AgentResultConsumer> logger)
    : DiscordConsumer<AgentResultContract>(connection, logger, gatewayClient)
{
    protected override void Configure()
    {
        ExchangeName = "agent.tasks";
        QueueName = "agent.tasks.results";
        RoutingKey = "agent.tasks.results";
    }

    protected override async Task HandleAsync(AgentResultContract body, CancellationToken ct = default)
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

        var responseContent = FormatResponse(body);
        var messageProperties = CreateMessageProperties(responseContent);

        var sentAsReply = await TryReplyAsync(channel, body.ChatContext.PromptMessageId, messageProperties, ct);
        if (!sentAsReply)
            await channel.SendMessageAsync(messageProperties, cancellationToken: ct);
    }

    private static string FormatResponse(AgentResultContract body)
    {
        var content = body.Content
            .Replace("@everyone", "{everyone_mention}")
            .Replace("@here", "{here_mention}");

        return $"{content}\n\n-# Tokens used: {body.TotalTokens}";
    }
}
using Bot.Infrastructure.Consumers.Common;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using RabbitMQ.Client;
using Shared.Common;
using Shared.Contracts;

namespace Bot.Infrastructure.Consumers;

public class AgentResultConsumer(
    IConnection connection,
    GatewayClient gatewayClient,
    ILogger<AgentResultConsumer> logger)
    : Consumer<AgentResultContract>(connection)
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

    private string FormatResponse(AgentResultContract body)
    {
        var content = body.Content
            .Replace("@everyone", "{everyone_mention}")
            .Replace("@here", "{here_mention}");

        return $"{content}\n\n-# Tokens used: {body.TotalTokens}";
    }

    private MessageProperties CreateMessageProperties(string content)
    {
        return new MessageProperties
        {
            Content = content,
            AllowedMentions = new AllowedMentionsProperties
            {
                Everyone = false,
                ReplyMention = false
            }
        };
    }

    private async Task<bool> TryReplyAsync(TextChannel channel, string? messageIdStr, MessageProperties props,
        CancellationToken ct = default)
    {
        if (!ulong.TryParse(messageIdStr, out var messageId))
        {
            logger.LogWarning("Invalid original message ID: {Id}", messageIdStr);
            return false;
        }

        try
        {
            var originalMessage = await channel.GetMessageAsync(messageId, cancellationToken: ct);

            var replyProps = new ReplyMessageProperties
            {
                Content = props.Content,
                Flags = props.Flags,
                AllowedMentions = props.AllowedMentions
            };
            
            logger.LogInformation("Replying to original message ID: {Id}", messageId);

            await originalMessage.ReplyAsync(replyProps, cancellationToken: ct);
            return true;
        }
        catch
        {
            // message not found
            return false;
        }
    }
}
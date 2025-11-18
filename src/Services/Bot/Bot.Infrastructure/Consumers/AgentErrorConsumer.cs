using Bot.Infrastructure.Consumers.Common;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using RabbitMQ.Client;
using Shared.Common;
using Shared.Contracts;

namespace Bot.Infrastructure.Consumers;

public class AgentErrorConsumer(IConnection connection, ILogger<AgentErrorConsumer> logger, GatewayClient gatewayClient)
    : Consumer<AgentErrorContract>(connection)
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
    
    private string FormatResponse(AgentErrorContract body)
    {
        var content = body.Message;
        if (content.Length > 100)
            content = content[..100];
        return $"{content}...";
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
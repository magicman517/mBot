using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using RabbitMQ.Client;
using Shared.Common;

namespace Bot.Infrastructure.Consumers.Common;

public abstract class DiscordConsumer<T>(IConnection connection, ILogger logger, GatewayClient gatewayClient) : Consumer<T>(connection, logger)
{
    protected async Task<bool> TryReplyAsync(TextChannel channel, string? messageIdStr, MessageProperties props,
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
            logger.LogWarning("Failed to reply to original message ID: {Id}", messageId);
            return false;
        }
    }
    
    protected bool TryGetContextIds(ChatContext context, out ulong guildId, out ulong channelId)
    {
        channelId = 0;
        return ulong.TryParse(context.GuildId, out guildId) &&
               ulong.TryParse(context.ChannelId, out channelId);
    }
    
    protected bool TryGetChannel(ulong guildId, ulong channelId, out TextChannel channel)
    {
        channel = null!;

        if (!gatewayClient.Cache.Guilds.TryGetValue(guildId, out var guild) ||
            !guild.Channels.TryGetValue(channelId, out var genericChannel) ||
            genericChannel is not TextChannel textChannel) return false;

        channel = textChannel;
        return true;
    }
    
    protected MessageProperties CreateMessageProperties(string content)
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
}
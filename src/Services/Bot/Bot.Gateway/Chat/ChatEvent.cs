using Contracts;
using JetBrains.Annotations;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using Wolverine;

namespace Bot.Gateway.Chat;

/// <summary>
/// Starts an agent when a message begins with the bot mention.
/// For text-like channels (text, stage, voice, announcement) include only the referenced (replied-to) message in the agent's context.
/// For threads, persist the full conversation history to the database to provide complete context.
/// </summary>
[UsedImplicitly]
public class ChatEvent(GatewayClient gatewayClient, IServiceProvider serviceProvider, ILogger<ChatEvent> logger)
    : IMessageCreateGatewayHandler
{
    private readonly string _mention = $"<@{gatewayClient.Id}>";

    public async ValueTask HandleAsync(Message message)
    {
        if (message.Author.IsBot
            || message.Guild is null
            || message.Author is not GuildUser user)
        {
            logger.LogDebug("Ignoring message from {User} in guild {Guild}",
                message.Author.Id, message.GuildId);
            return;
        }

        if (!message.Content.StartsWith(_mention, StringComparison.Ordinal))
        {
            logger.LogDebug("Ignoring message from {User} in guild {Guild} as it does not mention the bot",
                user.Id, message.Guild.Id);
            return;
        }

        var prompt = message.Content.Length > _mention.Length
            ? message.Content[_mention.Length..].Trim()
            : string.Empty;
        if (string.IsNullOrEmpty(prompt))
        {
            logger.LogDebug("Ignoring message from {User} in guild {Guild} as it contains no prompt", user.Id,
                message.Guild.Id);
            await message.AddReactionAsync("❓");
            return;
        }

        await using var scope = serviceProvider.CreateAsyncScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        
        await message.AddReactionAsync("👀");

        if (message.Channel is GuildThread threadChannel)
        {
            // TODO
        }
        else
        {
            // this could be stage, voice, announcement or text channel
            var contract = new StartAgentContract
            {
                Prompt = prompt,
                ReferencedMessageContent = message.ReferencedMessage?.Content,
                GuildId = message.Guild.Id,
                ChannelId = message.ChannelId,
                UserId = user.Id,
                UserName = user.Nickname ?? user.Username,
                IsAdmin = user.GetPermissions(message.Guild).HasFlag(Permissions.Administrator)
            };
            await bus.PublishAsync(contract);
            logger.LogInformation("Started agent for message from {User} in guild {Guild}",
                user.Id, message.Guild.Id);
        }
    }
}
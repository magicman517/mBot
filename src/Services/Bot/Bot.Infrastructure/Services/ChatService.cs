using Bot.Core.Interfaces.Services;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using Shared.Common;
using Shared.Contracts;

namespace Bot.Infrastructure.Services;

public class ChatService(GatewayClient gatewayClient, IPublisherService publisherService) : IChatService
{
    private readonly string _mention = $"<@{gatewayClient.Id}>";

    private string StripMention(string content)
    {
        return !content.StartsWith(_mention, StringComparison.OrdinalIgnoreCase)
            ? content
            : content[_mention.Length..].Trim();
    }

    public async Task StartAgentAsync(Message message, TextChannel? textChannel, CancellationToken ct = default)
    {
        if (textChannel is null)
            return;

        var prompt = StripMention(message.Content);
        if (string.IsNullOrWhiteSpace(prompt))
        {
            await message.AddReactionAsync("❓", cancellationToken: ct);
            return;
        }

        await message.AddReactionAsync("👀", cancellationToken: ct);

        var chatContext = new ChatContext
        {
            GuildId = message.Guild!.Id.ToString(),
            ChannelId = textChannel.Id.ToString(),
            UserId = message.Author.Id.ToString(),
            PromptMessageId = message.Id.ToString()
        };
        var contract = new AgentStartContract
        {
            Stream = false,
            BotId = gatewayClient.Id.ToString(),
            ChatContext = chatContext,
            Prompt = prompt,
            ReferencedMessage = message.ReferencedMessage is not null
                ? new ReferencedMessage
                {
                    Id = message.ReferencedMessage.Id.ToString(),
                    Content = message.ReferencedMessage.Content
                }
                : null
        };

        await publisherService.PublishAsync("agent.tasks", "agent.tasks", contract, ct);
    }

    public async Task StartThreadAgentAsync(Message message, GuildThread thread, CancellationToken ct = default)
    {
        await message.ReplyAsync(new ReplyMessageProperties
        {
            Content = "Threads are not supported yet",
            Flags = MessageFlags.SuppressNotifications
        }, cancellationToken: ct);
    }
}
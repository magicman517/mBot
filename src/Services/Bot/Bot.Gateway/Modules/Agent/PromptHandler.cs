using Bot.Core.Interfaces.Services;
using JetBrains.Annotations;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace Bot.Gateway.Modules.Agent;

[UsedImplicitly]
public class PromptHandler(IChatService chatService, GatewayClient gatewayClient) : IMessageCreateGatewayHandler
{
    private readonly string _mention = $"<@{gatewayClient.Id}>";
    
    public async ValueTask HandleAsync(Message message)
    {
        if (ShouldIgnore())
            return;

        switch (message.Channel)
        {
            case GuildThread thread:
                await chatService.StartThreadAgentAsync(message, thread);
                break;
            default:
                // this could be text, voice, announcement, etc.
                if (IsMentioned())
                    await chatService.StartAgentAsync(message, message.Channel);
                break;
        }

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        bool ShouldIgnore()
            => message.Author.IsBot || message.Guild is null;
        
        bool IsMentioned()
            => message.Content.StartsWith(_mention, StringComparison.OrdinalIgnoreCase);
    }
}
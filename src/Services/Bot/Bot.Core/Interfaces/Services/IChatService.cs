using NetCord;
using NetCord.Gateway;

namespace Bot.Core.Interfaces.Services;

public interface IChatService
{
    Task StartAgentAsync(Message message, TextChannel? textChannel, CancellationToken ct = default);
    Task StartThreadAgentAsync(Message message, GuildThread thread, CancellationToken ct = default);
}
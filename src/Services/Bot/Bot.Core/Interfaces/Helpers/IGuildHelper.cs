using NetCord;
using NetCord.Gateway;

namespace Bot.Core.Interfaces.Helpers;

public interface IGuildHelper
{
    Guild? GetGuildById(string guildId);
    Task<GuildUser?> GetUserByIdAsync(string guildId, string userId);

    IGuildChannel? GetChannelById(Guild guild, string channelId);
    Role? GetRoleById(Guild guild, string roleId);
}
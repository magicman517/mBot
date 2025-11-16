using Bot.Core.Interfaces.Helpers;
using NetCord;
using NetCord.Gateway;

namespace Bot.Application.Helpers;

public class GuildHelper(GatewayClient gatewayClient) : IGuildHelper
{
    public Guild? GetGuildById(string guildId)
    {
        if (!ulong.TryParse(guildId, out var gId))
            return null;

        var guild = gatewayClient.Cache.Guilds.Values.FirstOrDefault(g => g.Id == gId);
        return guild;
    }

    public async Task<GuildUser?> GetUserByIdAsync(string guildId, string userId)
    {
        var guild = GetGuildById(guildId);
        if (guild == null)
            return null;

        if (!ulong.TryParse(userId, out var uId))
            return null;

        var guildUser = guild.Users.Values.FirstOrDefault(u => u.Id == uId);
        if (guildUser is null)
        {
            try
            {
                guildUser = await guild.GetUserAsync(uId);
            }
            catch
            {
                return null;
            }
        }
        return guildUser;
    }

    public IGuildChannel? GetChannelById(Guild guild, string channelId)
    {
        if (!ulong.TryParse(channelId, out var cId))
            return null;

        var channel = guild.Channels.Values.FirstOrDefault(c => c.Id == cId);
        return channel;
    }

    public Role? GetRoleById(Guild guild, string roleId)
    {
        if (!ulong.TryParse(roleId, out var rId))
            return null;

        var role = guild.Roles.Values.FirstOrDefault(r => r.Id == rId);
        return role;
    }
}
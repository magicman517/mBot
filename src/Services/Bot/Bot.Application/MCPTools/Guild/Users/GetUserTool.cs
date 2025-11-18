using System.ComponentModel;
using Bot.Application.MCPTools.Guild.Users.DTOs;
using Bot.Core.Interfaces.Helpers;
using JetBrains.Annotations;
using ModelContextProtocol.Server;
using NetCord;

namespace Bot.Application.MCPTools.Guild.Users;

[McpServerToolType]
public class GetUserTool(IGuildHelper guildHelper)
{
    [UsedImplicitly]
    [McpServerTool(Name = "guild_get_user"),
    Description("Get a user in a guild by their ID. " +
                "Returns the user's ID, username, roles, bot status, owner status, admin status, and join date.")]
    public async Task<ToolResult<UserDto>> GetUser(string guildId, string userId)
    {
        var guild = guildHelper.GetGuildById(guildId);
        if (guild is null)
            return "Guild not found";
        
        var user = await guildHelper.GetUserByIdAsync(guildId, userId);
        return user is null
            ? "User not found"
            : new UserDto
            {
                Id = user.Id.ToString(),
                Username = user.Nickname ?? user.Username,
                Roles = user.GetRoles(guild).Select(r => r.Id.ToString()).ToArray(),
                IsBot = user.IsBot,
                IsOwner = guild.OwnerId == user.Id,
                IsAdmin = user.GetPermissions(guild).HasFlag(Permissions.Administrator),
                JoinedAt = user.JoinedAt
            };
    }
}
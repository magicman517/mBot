using System.ComponentModel;
using Bot.Application.MCPTools.Guild.General.DTOs;
using Bot.Core.Interfaces.Helpers;
using JetBrains.Annotations;
using ModelContextProtocol.Server;

namespace Bot.Application.MCPTools.Guild.General;

[McpServerToolType]
public class GetGuildTool(IGuildHelper guildHelper)
{
    [UsedImplicitly]
    [McpServerTool(Name = "guild_get_info"),
    Description("Gets public information about a guild by its ID. " +
                "Returns the guild's name, creation date, owner ID, premium tier, and user count. " +
                "Note: user count may not be accurate.")]
    public ToolResult<GuildDto> GetGuild(string guildId)
    {
        var guild = guildHelper.GetGuildById(guildId);

        return guild is null
            ? "Guild not found"
            : new GuildDto
            {
                Id = guild.Id.ToString(),
                Name = guild.Name,
                UserCount = guild.UserCount,
                CreatedAt = guild.CreatedAt,
                OwnerId = guild.OwnerId.ToString(),
                PremiumTier = guild.PremiumTier
            };
    }
}
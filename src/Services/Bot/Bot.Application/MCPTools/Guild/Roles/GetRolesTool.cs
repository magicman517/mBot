using System.ComponentModel;
using Bot.Application.MCPTools.Guild.Roles.DTOs;
using Bot.Core.Interfaces.Helpers;
using JetBrains.Annotations;
using ModelContextProtocol.Server;

namespace Bot.Application.MCPTools.Guild.Roles;

[McpServerToolType]
public class GetRolesTool(IGuildHelper guildHelper)
{
    [UsedImplicitly]
    [McpServerTool(Name = "guild_get_roles"),
    Description("Get all roles in a guild by its ID. " +
                "Returns an array of roles with their ID, name, color, position, permissions, and whether they are managed.")]
    public ToolResult<IEnumerable<RoleDto>> GetRoles(string guildId)
    {
        var guild = guildHelper.GetGuildById(guildId);
        
        return guild is null
            ? "Guild not found"
            : guild.Roles.Values.Select(r => new RoleDto
            {
                Id = r.Id.ToString(),
                Name = r.Name,
                Color = r.Color.ToString(),
                Position = r.Position,
                Permissions = r.Permissions.ToString(),
                IsManaged = r.Managed
            }).ToArray();
    }
}
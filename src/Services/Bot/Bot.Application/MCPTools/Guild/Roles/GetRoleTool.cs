using System.ComponentModel;
using Bot.Application.MCPTools.Guild.Roles.DTOs;
using Bot.Core.Interfaces.Helpers;
using JetBrains.Annotations;
using ModelContextProtocol.Server;

namespace Bot.Application.MCPTools.Guild.Roles;

[McpServerToolType]
public class GetRoleTool(IGuildHelper guildHelper)
{
    [UsedImplicitly]
    [McpServerTool(Name = "guild_get_role"),
     Description("Get a role in a guild by its ID. " +
                 "Returns the role with its ID, name, color, position, permissions, and whether it is managed.")]
    public ToolResult<RoleDto> GetRole(string guildId, string roleId)
    {
        var guild = guildHelper.GetGuildById(guildId);
        if (guild is null)
            return "Guild not found";

        var role = guildHelper.GetRoleById(guild, roleId);
        return role is null
            ? "Role not found"
            : new RoleDto
            {
                Id = role.Id.ToString(),
                Name = role.Name,
                Color = role.Color.ToString(),
                Position = role.Position,
                Permissions = role.Permissions.ToString(),
                IsManaged = role.Managed
            };
    }
}
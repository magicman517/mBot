using System.ComponentModel;
using Bot.Application.MCPTools.Guild.Channels.DTOs;
using Bot.Core.Interfaces.Helpers;
using JetBrains.Annotations;
using ModelContextProtocol.Server;

namespace Bot.Application.MCPTools.Guild.Channels;

[McpServerToolType]
public class GetChannelsTool(IGuildHelper guildHelper)
{
    [UsedImplicitly]
    [McpServerTool(Name = "guild_get_channels"),
    Description("Get all channels in a guild by its ID. " +
                "Returns an array of channels with their ID, name, type, position, and creation date.")]
    public ToolResult<IEnumerable<ChannelDto>> GetChannels(string guildId)
    {
        var guild = guildHelper.GetGuildById(guildId);
        return guild is null
            ? "Guild not found"
            : guild.Channels.Values.Select(c => new ChannelDto
            {
                Id = c.Id.ToString(),
                Name = c.Name,
                Type = c.GetType().Name,
                Position = c.Position,
                CreatedAt = c.CreatedAt
            }).ToArray();
    }
}
using System.ComponentModel;
using Bot.Application.MCPTools.Guild.Channels.DTOs;
using Bot.Core.Interfaces.Helpers;
using JetBrains.Annotations;
using ModelContextProtocol.Server;

namespace Bot.Application.MCPTools.Guild.Channels;

[McpServerToolType]
public class GetChannelTool(IGuildHelper guildHelper)
{
    [UsedImplicitly]
    [McpServerTool(Name = "guild_get_channel"),
    Description("Get a channel in a guild by its ID. " +
                "Returns the channel with its ID, name, type, position, and creation date.")]
    public ToolResult<ChannelDto> GetChannel(string guildId, string channelId)
    {
        var guild = guildHelper.GetGuildById(guildId);
        if (guild is null)
            return "Guild not found";
        
        var channel = guildHelper.GetChannelById(guild, channelId);
        return channel is null
            ? "Channel not found"
            : new ChannelDto
            {
                Id = channel.Id.ToString(),
                Name = channel.Name,
                Type = channel.GetType().Name,
                Position = channel.Position,
                CreatedAt = channel.CreatedAt
            };
    }
}
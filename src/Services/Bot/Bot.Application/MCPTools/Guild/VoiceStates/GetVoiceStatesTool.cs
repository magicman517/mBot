using System.ComponentModel;
using Bot.Application.MCPTools.Guild.VoiceStates.DTOs;
using Bot.Core.Interfaces.Helpers;
using JetBrains.Annotations;
using ModelContextProtocol.Server;

namespace Bot.Application.MCPTools.Guild.VoiceStates;

[McpServerToolType]
public class GetVoiceStatesTool(IGuildHelper guildHelper)
{
    [UsedImplicitly]
    [McpServerTool(Name = "guild_get_voice_states"),
     Description("Get all voice states in a guild by its ID. " +
                 "Returns an array of voice states with their user ID, channel ID, and various voice state flags.")]
    public ToolResult<IEnumerable<VoiceStateDto>> GetVoiceStates(string guildId)
    {
        var guild = guildHelper.GetGuildById(guildId);
        return guild is null
            ? "Guild not found"
            : guild.VoiceStates.Values.Select(vs => new VoiceStateDto
            {
                UserId = vs.UserId.ToString(),
                ChannelId = vs.ChannelId.ToString(),
                IsDeafened = vs.IsDeafened,
                IsMuted = vs.IsMuted,
                IsSelfDeafened = vs.IsSelfDeafened,
                IsSelfMuted = vs.IsSelfMuted,
                IsStreaming = vs.SelfStreamExists.GetValueOrDefault(),
                IsSuppressed = vs.Suppressed,
                IsVideoEnabled = vs.SelfVideoExists
            }).ToArray();
    }
}
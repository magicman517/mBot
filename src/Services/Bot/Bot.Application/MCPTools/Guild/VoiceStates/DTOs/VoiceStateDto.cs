namespace Bot.Application.MCPTools.Guild.VoiceStates.DTOs;

public class VoiceStateDto
{
    public string? UserId { get; init; }
    public string? ChannelId { get; init; }
    public bool IsDeafened { get; init; }
    public bool IsMuted { get; init; }
    public bool IsSelfDeafened { get; init; }
    public bool IsSelfMuted { get; init; }
    public bool IsStreaming { get; init; }
    public bool IsSuppressed { get; init; }
    public bool IsVideoEnabled { get; init; }
}
namespace Bot.Application.MCPTools.Guild.Channels.DTOs;

public record ChannelDto
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Type { get; init; }
    public int? Position { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }
}
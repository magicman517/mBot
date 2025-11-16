namespace Bot.Application.MCPTools.Guild.General.DTOs;

public record GuildDto
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public int? UserCount { get; init; }
    public string? OwnerId { get; init; }
    public int? PremiumTier { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }
}
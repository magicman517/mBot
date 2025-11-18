namespace Bot.Application.MCPTools.Guild.Users.DTOs;

public record UserDto
{
    public string? Id { get; init; }
    public string? Username { get; init; }
    public IEnumerable<string>? Roles { get; init; }
    public bool IsBot { get; init; }
    public bool IsAdmin { get; init; }
    public bool IsOwner { get; init; }
    public DateTimeOffset? JoinedAt { get; init; }
}
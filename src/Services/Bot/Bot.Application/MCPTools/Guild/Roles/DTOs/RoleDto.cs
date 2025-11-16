namespace Bot.Application.MCPTools.Guild.Roles.DTOs;

public record RoleDto
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Color { get; init; }
    public int? Position { get; init; }
    public string? Permissions { get; init; }
    public bool IsManaged { get; init; }
}
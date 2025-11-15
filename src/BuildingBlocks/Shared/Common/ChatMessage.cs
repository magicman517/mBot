using System.Text.Json.Serialization;

namespace Shared.Common;

/// <summary>
/// Represents a message in a chat conversation with a role and content
/// </summary>
public record ChatMessage
{
    /// <summary>
    /// Gets the role of the message sender (e.g., "user", "assistant", "system")
    /// </summary>
    [JsonPropertyName("role")]
    public required string Role { get; init; }
    
    /// <summary>
    /// Gets the content of the message
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; init; }
}
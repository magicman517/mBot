using System.Text.Json.Serialization;

namespace Shared.Common;

public record ReferencedMessage
{
    /// <summary>
    /// Gets the ID of a referenced/replied-to message
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    /// <summary>
    /// Gets the content of a referenced/replied-to message
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; init; }
}
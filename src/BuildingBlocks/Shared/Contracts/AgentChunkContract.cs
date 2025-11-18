using System.Text.Json.Serialization;
using Shared.Common;

namespace Shared.Contracts;

public record AgentChunkContract
{
    [JsonPropertyName("chat_context")]
    public required ChatContext ChatContext { get; init; }
    
    [JsonPropertyName("content")]
    public required string Content { get; init; }
}
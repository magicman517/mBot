using System.Text.Json.Serialization;
using Shared.Common;

namespace Shared.Contracts;

public record AgentResultContract
{
    [JsonPropertyName("chat_context")]
    public required ChatContext ChatContext { get; init; }
    
    [JsonPropertyName("content")]
    public required string Content { get; init; }
    
    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}
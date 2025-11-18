using System.Text.Json.Serialization;
using Shared.Common;

namespace Shared.Contracts;

public record AgentErrorContract
{
    [JsonPropertyName("chat_context")]
    public required ChatContext ChatContext { get; init; }
    
    [JsonPropertyName("message")]
    public required string Message { get; init; }
}
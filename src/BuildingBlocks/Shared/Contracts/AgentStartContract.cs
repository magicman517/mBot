using System.Text.Json.Serialization;
using Shared.Common;

namespace Shared.Contracts;

/// <summary>
/// Contract for initiating an agent interaction in response to a user message
/// </summary>
public record AgentStartContract
{
    /// <summary>
    /// Indicates whether the agent's response should be streamed back to the user
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; init; }
    
    /// <summary>
    /// Gets the Discord bot ID handling the interaction
    /// </summary>
    [JsonPropertyName("bot_id")]
    public required string BotId { get; init; }
    
    /// <summary>
    /// Gets the chat context including guild, channel, and user information
    /// </summary>
    [JsonPropertyName("chat_context")]
    public required ChatContext ChatContext { get; init; }
    
    /// <summary>
    /// Gets the user's prompt or message content
    /// </summary>
    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }
    
    /// <summary>
    /// Gets information about any referenced or replied-to message
    /// </summary>
    [JsonPropertyName("referenced_message")]
    public ReferencedMessage? ReferencedMessage { get; init; }

    /// <summary>
    /// Gets the chat history for context
    /// </summary>
    [JsonPropertyName("chat_history")]
    public IEnumerable<ChatMessage> ChatHistory { get; init; } = [];
}
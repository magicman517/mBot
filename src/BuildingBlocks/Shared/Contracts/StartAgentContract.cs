using System.Text.Json.Serialization;
using Shared.Common;

namespace Shared.Contracts;

/// <summary>
/// Contract for initiating an agent interaction in response to a user message
/// </summary>
public record StartAgentContract
{
    /// <summary>
    /// Gets the user's prompt or message content
    /// </summary>
    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }
    
    /// <summary>
    /// Gets the content of a referenced/replied-to message, if any
    /// </summary>
    [JsonPropertyName("referenced_message_content")]
    public string? ReferencedMessageContent { get; init; }
    
    /// <summary>
    /// Gets the chat history for context
    /// </summary>
    [JsonPropertyName("chat_history")]
    public IEnumerable<ChatMessage> ChatHistory { get; init; } = [];
    
    /// <summary>
    /// Gets the Discord guild (server) ID where the message originated
    /// </summary>
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; init; }
    
    /// <summary>
    /// Gets the Discord channel ID where the message originated
    /// </summary>
    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; init; }
    
    /// <summary>
    /// Gets the Discord user ID of the message author
    /// </summary>
    [JsonPropertyName("user_id")]
    public required ulong UserId { get; init; }
    
    /// <summary>
    /// Gets the Discord username or nickname of the message author
    /// </summary>
    [JsonPropertyName("user_name")]
    public required string UserName { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether the user has administrator permissions
    /// </summary>
    [JsonPropertyName("is_admin")]
    public bool IsAdmin { get; init; }
}
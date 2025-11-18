using System.Text.Json.Serialization;

namespace Shared.Common;

public record ChatContext
{
    /// <summary>
    /// Gets the Discord guild (server) ID where the message originated
    /// </summary>
    [JsonPropertyName("guild_id")]
    public required string GuildId { get; init; }
    
    /// <summary>
    /// Gets the Discord channel ID where the message originated
    /// </summary>
    [JsonPropertyName("channel_id")]
    public required string ChannelId { get; init; }
    
    /// <summary>
    /// Gets the Discord user ID of the message author
    /// </summary>
    [JsonPropertyName("user_id")]
    public required string UserId { get; init; }
    
    /// <summary>
    /// Gets the ID of the prompt message that initiated the chat. Only applicable for chats without history
    /// </summary>
    [JsonPropertyName("prompt_message_id")]
    public string? PromptMessageId { get; init; }
}
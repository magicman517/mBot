using Bot.Core.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bot.Core.Entities;

public class Guild : BaseEntity
{
    public required ulong GuildId { get; set; }
    public required string Name { get; set; }
    public AgentConfiguration AgentConfiguration { get; set; } = new();
}

public class AgentConfiguration
{
    [BsonRepresentation(BsonType.String)]
    public Provider Provider { get; set; } = Provider.OpenAi;
    public string Model { get; set; } = "gpt-5-mini";
    
    public string? ApiKey { get; set; }
    public string? SystemInstruction { get; set; }
}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bot.Core.Common;

[BsonIgnoreExtraElements]
public abstract class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    
    [BsonElement("_created")]
    public DateTime Created { get; set; } = DateTime.UtcNow;
    
    [BsonElement("_updated")]
    public DateTime Updated { get; set; } = DateTime.UtcNow;
    
    [BsonElement("_version")]
    public int Version { get; set; } = 1;
}
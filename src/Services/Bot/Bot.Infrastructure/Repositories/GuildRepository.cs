using Bot.Core.Entities;
using Bot.Core.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bot.Infrastructure.Repositories;

public class GuildRepository : IGuildRepository
{
    private readonly IMongoCollection<Guild> _collection;

    public GuildRepository(IMongoClient client)
    {
        _collection = client
            .GetDatabase("mBot")
            .GetCollection<Guild>("guilds");
        
        var indexKeys = Builders<Guild>.IndexKeys.Ascending(g => g.GuildId);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<Guild>(indexKeys, indexOptions);
        _collection.Indexes.CreateOne(indexModel);
    }
    
    public async Task<Guild?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var objectId = ObjectId.Parse(id);
        var filter = Builders<Guild>.Filter.Eq(g => g.Id, objectId);
        return await _collection.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task<Guild?> GetByIdAsync(ObjectId id, CancellationToken ct = default)
    {
        var filter = Builders<Guild>.Filter.Eq(g => g.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task<Guild?> GetByGuildIdAsync(ulong id, CancellationToken ct = default)
    {
        var filter = Builders<Guild>.Filter.Eq(g => g.GuildId, id);
        return await _collection.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task CreateAsync(Guild entity, CancellationToken ct = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: ct);
    }

    public async Task UpdateAsync(ulong id, Func<UpdateDefinitionBuilder<Guild>, UpdateDefinition<Guild>> builder, CancellationToken ct = default)
    {
        var filter = Builders<Guild>.Filter.Eq(g => g.GuildId, id);
        var update = builder(Builders<Guild>.Update);
        await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
    }
}
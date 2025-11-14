using Bot.Core.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bot.Core.Interfaces.Repositories;

public interface IGuildRepository
{
    Task<Guild?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Guild?> GetByIdAsync(ObjectId id, CancellationToken ct = default);
    Task<Guild?> GetByGuildIdAsync(ulong id, CancellationToken ct = default);
    
    Task CreateAsync(Guild entity, CancellationToken ct = default);
    Task UpdateAsync(ulong id, Func<UpdateDefinitionBuilder<Guild>, UpdateDefinition<Guild>> builder, CancellationToken ct = default);
}
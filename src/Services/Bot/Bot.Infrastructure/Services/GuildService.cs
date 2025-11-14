using Bot.Core.Common;
using Bot.Core.Entities;
using Bot.Core.Interfaces.Repositories;
using Bot.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Bot.Infrastructure.Services;

public class GuildService(IGuildRepository repository, ILogger<GuildService> logger) : IGuildService
{
    public async Task<Guild?> GetGuildAsync(ulong id, CancellationToken ct = default)
    {
        return await repository.GetByGuildIdAsync(id, ct);
    }

    public async Task CreateGuildAsync(ulong id, string name, CancellationToken ct = default)
    {
        try
        {
            var guild = new Guild
            {
                GuildId = id,
                Name = name,
            };
            await repository.CreateAsync(guild, ct);
            logger.LogInformation("Created guild {GuildId} with name {GuildName}", id, name);
        }
        catch (MongoWriteException e)
        {
            if (e.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                logger.LogWarning("Guild {GuildId} already exists, skipping creation", id);
            }
            else
            {
                logger.LogError(e, "Error creating guild {GuildId}", id);
            }
        }
    }

    public async Task UpdateAgentConfigurationAsync(ulong id, Provider? provider, string? model, string? apiKey,
        CancellationToken ct = default)
    {
        // TODO: encrypt apiKey before storing
        await repository.UpdateAsync(id, u =>
        {
            var updates = new List<UpdateDefinition<Guild>>();
            
            if (provider is not null) updates.Add(u.Set(g => g.AgentConfiguration.Provider, provider.Value));
            if (model is not null) updates.Add(u.Set(g => g.AgentConfiguration.Model, model));
            if (apiKey is not null) updates.Add(u.Set(g => g.AgentConfiguration.ApiKey, apiKey));

            return updates.Count > 0
                ? u.Combine(updates)
                : u.Combine();
        }, ct);
        logger.LogInformation("Updated {GuildId}", id);
    }
}
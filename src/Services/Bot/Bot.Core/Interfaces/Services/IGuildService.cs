using Bot.Core.Common;
using Bot.Core.Entities;

namespace Bot.Core.Interfaces.Services;

public interface IGuildService
{
    Task<Guild?> GetGuildAsync(ulong id, CancellationToken ct = default);
    
    Task CreateGuildAsync(ulong id, string name, CancellationToken ct = default);
    Task UpdateAgentConfigurationAsync(ulong id, Provider? provider, string? model, string? apiKey, CancellationToken ct = default);
}
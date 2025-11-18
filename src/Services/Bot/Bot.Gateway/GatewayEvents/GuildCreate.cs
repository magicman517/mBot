using Bot.Core.Interfaces.Services;
using JetBrains.Annotations;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace Bot.Gateway.GatewayEvents;

[UsedImplicitly]
public class GuildCreate(IGuildService guildService) : IGuildCreateGatewayHandler
{
    public async ValueTask HandleAsync(GuildCreateEventArgs guild)
    {
        await guildService.CreateGuildAsync(guild.GuildId, guild.Guild?.Name ?? "Name unavailable");
    }
}
using JetBrains.Annotations;
using NetCord;
using NetCord.Services.ApplicationCommands;

namespace Bot.Gateway.Modules.ApplicationCommands;

/// <summary>
/// Admin only command to configure agent
/// </summary>
[UsedImplicitly]
public class Configuration : ApplicationCommandModule<ApplicationCommandContext>
{
    [UsedImplicitly]
    [SlashCommand("configuration", "Configure the agent for this guild",
        DefaultGuildPermissions = Permissions.Administrator, Contexts = [InteractionContextType.Guild])]
    public async Task ConfigureAsync()
    {
    }
}
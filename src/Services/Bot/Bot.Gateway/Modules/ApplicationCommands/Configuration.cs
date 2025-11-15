using JetBrains.Annotations;
using NetCord;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace Bot.Gateway.Modules.ApplicationCommands;

[UsedImplicitly]
[SlashCommand("config", "Configuration commands for the agent", Contexts = [InteractionContextType.Guild])]
public class Configuration : ApplicationCommandModule<ApplicationCommandContext>
{
    [UsedImplicitly]
    [SubSlashCommand("get", "Get the current agent configuration")]
    public string GetConfiguration()
    {
        return "WIP";
    }

    [UsedImplicitly]
    [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
    [SubSlashCommand("set", "Update the agent configuration")]
    public string SetConfiguration()
    {
        return "WIP";
    }
}
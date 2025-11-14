using System.Reflection;
using Bot.Core.Interfaces.Repositories;
using Bot.Core.Interfaces.Services;
using Bot.Infrastructure.Repositories;
using Bot.Infrastructure.Services;
using Contracts;
using JasperFx.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Services.ComponentInteractions;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Bot.Infrastructure;

public static class Extensions
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddMongoDBClient("mongodb");

        builder.Services.AddDiscordGateway(options =>
            {
                options.Intents = GatewayIntents.All;
            })
            .AddGatewayHandlers(Assembly.GetCallingAssembly())
            .AddApplicationCommands()
            .AddComponentInteractions<ModalInteraction, ModalInteractionContext>();
        
        builder.UseWolverine(options =>
        {
            options
                .UseRabbitMqUsingNamedConnection("rabbitmq")
                .DisableDeadLetterQueueing()
                .AutoProvision();

            options.PublishMessage<StartAgentContract>()
                .ToRabbitExchange("agent.tasks", cfg => { cfg.BindQueue("agent.start"); });

            options.Services.AddResourceSetupOnStartup();
        });

        builder.Services.AddSingleton<IGuildRepository, GuildRepository>();
        builder.Services.AddSingleton<IGuildService, GuildService>();
    }
}
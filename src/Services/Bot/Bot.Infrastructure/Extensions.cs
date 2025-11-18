using System.Reflection;
using Bot.Core.Interfaces.Repositories;
using Bot.Core.Interfaces.Services;
using Bot.Infrastructure.ChannelPool;
using Bot.Infrastructure.Consumers;
using Bot.Infrastructure.Repositories;
using Bot.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Services.ComponentInteractions;

namespace Bot.Infrastructure;

public static class Extensions
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        if (builder.Environment.EnvironmentName != "Testing")
        {
            builder.Services.AddDiscordGateway(options => { options.Intents = GatewayIntents.All; })
                .AddGatewayHandlers(Assembly.GetCallingAssembly())
                .AddApplicationCommands()
                .AddComponentInteractions<ModalInteraction, ModalInteractionContext>();
        }

        builder.AddMongoDBClient("mongodb");
        builder.AddRabbitMQClient("rabbitmq");

        builder.Services.AddSingleton<IAsyncChannelPool, AsyncChannelPool>();
        builder.Services.AddSingleton<IPublisherService, PublisherService>();

        builder.Services.AddHostedService<AgentChunkConsumer>();
        builder.Services.AddHostedService<AgentResultConsumer>();
        builder.Services.AddHostedService<AgentErrorConsumer>();

        builder.Services.AddSingleton<IGuildRepository, GuildRepository>();
        builder.Services.AddSingleton<IGuildService, GuildService>();
        builder.Services.AddSingleton<IChatService, ChatService>();
    }
}
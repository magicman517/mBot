using System.Reflection;
using Bot.Application.Services;
using Bot.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bot.Application;

public static class Extensions
{
    public static void AddApplication(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly(Assembly.GetExecutingAssembly());
        
        builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
    }
}
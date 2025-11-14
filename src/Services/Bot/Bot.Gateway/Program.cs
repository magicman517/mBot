using System.Reflection;
using Bot.Application;
using Bot.Infrastructure;
using NetCord.Hosting.Services;
using ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddApplication();
builder.AddInfrastructure();

var host = builder.Build();

host.AddModules(Assembly.GetExecutingAssembly());

await host.RunAsync();
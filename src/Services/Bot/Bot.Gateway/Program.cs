using System.Reflection;
using Bot.Application;
using Bot.Infrastructure;
using NetCord.Hosting.Services;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplication();
builder.AddInfrastructure();

var app = builder.Build();

app.AddModules(Assembly.GetExecutingAssembly());

app.MapMcp();

app.MapGet("/healthz", () => Results.Ok());

await app.RunAsync();
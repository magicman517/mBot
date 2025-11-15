using Aspire.Hosting;
using Bot.Application;
using Bot.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IntegrationTests;

[CollectionDefinition("AspireApp")]
public class AspireAppCollection : ICollectionFixture<AspireFixture>;

public class AspireFixture : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
    private IDistributedApplicationTestingBuilder? _appHost;

    public DistributedApplication App { get; private set; } = null!;
    public IServiceProvider Services { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        _appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>(
        [
            "--environment=Testing"
        ]);
        _appHost.Services.AddLogging(logging => { logging.SetMinimumLevel(LogLevel.Debug); });
        _appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        App = await _appHost.BuildAsync().WaitAsync(DefaultTimeout);
        await App.StartAsync().WaitAsync(DefaultTimeout);

        await App.ResourceNotifications.WaitForResourceHealthyAsync("mongodb").WaitAsync(DefaultTimeout);
        await App.ResourceNotifications.WaitForResourceHealthyAsync("rabbitmq").WaitAsync(DefaultTimeout);

        var testHostBuilder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
            Args = []
        });

        testHostBuilder.Configuration["Encryption:Key"] = "Iy+Y5F3IgCNUlY6st7sJhmzK3Wt6n6d9OV9rrDOsYgg=";

        var mongodbConnectionString = await App.GetConnectionStringAsync("mongodb");
        var rabbitmqConnectionString = await App.GetConnectionStringAsync("rabbitmq");

        testHostBuilder.Configuration["ConnectionStrings:mongodb"] = mongodbConnectionString;
        testHostBuilder.Configuration["ConnectionStrings:rabbitmq"] = rabbitmqConnectionString;

        testHostBuilder.AddApplication();
        testHostBuilder.AddInfrastructure();

        var testHost = testHostBuilder.Build();
        Services = testHost.Services;
    }

    public async ValueTask DisposeAsync()
    {
        await App.DisposeAsync();

        if (_appHost is not null)
        {
            await _appHost.DisposeAsync();
        }
    }
}
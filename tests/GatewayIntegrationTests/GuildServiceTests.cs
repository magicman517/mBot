using Bot.Core.Common;
using Bot.Core.Interfaces.Services;

namespace IntegrationTests;

[Collection("AspireApp")]
public class GuildServiceTests(AspireFixture aspire)
{
    [Fact]
    public async Task GetGuild_NonExistentGuild_ReturnsNull()
    {
        // Arrange
        await using var scope = aspire.Services.CreateAsyncScope();
        var guildService = scope.ServiceProvider.GetRequiredService<IGuildService>();

        const ulong nonExistentGuildId = 111111111111111111;

        // Act
        var retrieved = await guildService.GetGuildAsync(nonExistentGuildId, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(retrieved);
    }
    
    [Fact]
    public async Task CreateGuild_PersistsGuildInDatabase()
    {
        // Arrange
        await using var scope = aspire.Services.CreateAsyncScope();
        var guildService = scope.ServiceProvider.GetRequiredService<IGuildService>();

        const ulong guildId = 123456789012345678;
        const string guildName = "Test Guild";

        // Act
        await guildService.CreateGuildAsync(guildId, guildName, TestContext.Current.CancellationToken);

        // Assert
        var retrieved = await guildService.GetGuildAsync(guildId, TestContext.Current.CancellationToken);
        Assert.NotNull(retrieved);
        Assert.Equal(guildId, retrieved.GuildId);
        Assert.Equal(guildName, retrieved.Name);
    }

    [Fact]
    public async Task UpdateGuildAgentConfiguration_EncryptsApiKey()
    {
        // Arrange
        await using var scope = aspire.Services.CreateAsyncScope();
        var guildService = scope.ServiceProvider.GetRequiredService<IGuildService>();

        const ulong guildId = 987654321098765432;
        const string guildName = "Agent Config Guild";
        const string apiKey = "super-secret-api-key";

        await guildService.CreateGuildAsync(guildId, guildName, TestContext.Current.CancellationToken);

        // Act
        await guildService.UpdateAgentConfigurationAsync(guildId, Provider.Mistral, "mistral-small-latest", apiKey,
            TestContext.Current.CancellationToken);
        
        // Assert
        var retrieved = await guildService.GetGuildAsync(guildId, TestContext.Current.CancellationToken);
        Assert.NotNull(retrieved);
        Assert.Equal(guildId, retrieved.GuildId);
        Assert.Equal(guildName, retrieved.Name);
        Assert.NotNull(retrieved.AgentConfiguration);
        Assert.Equal(Provider.Mistral, retrieved.AgentConfiguration.Provider);
        Assert.Equal("mistral-small-latest", retrieved.AgentConfiguration.Model);
        Assert.NotEqual(apiKey, retrieved.AgentConfiguration.ApiKey); // Should be encrypted
    }
}
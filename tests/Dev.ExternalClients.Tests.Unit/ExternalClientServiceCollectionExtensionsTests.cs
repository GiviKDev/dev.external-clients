using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Dev.ExternalClients.Tests.Unit;

public class ExternalClientServiceCollectionExtensionsTests
{
    [Fact]
    public void AddExternalClient_WithActionConfiguration_ConfiguresHttpClientProperly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddExternalClient<IActionTestClient, ActionTestClient>(opts =>
        {
            opts.BaseAddress = "http://example.com";
            opts.Timeout = TimeSpan.FromSeconds(5);
            opts.DefaultRequestHeaders = new Dictionary<string, string>
            {
                { "Test-Header", "Value" }
            };
        });
        var provider = services.BuildServiceProvider();

        // Act
        var client = provider.GetRequiredService<IActionTestClient>() as ActionTestClient;

        // Assert
        Assert.NotNull(client);
        Assert.Equal(new Uri("http://example.com"), client.HttpClient.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(5), client.HttpClient.Timeout);
        Assert.True(client.HttpClient.DefaultRequestHeaders.Contains("Test-Header"));
        Assert.Equal("Value", client.HttpClient.DefaultRequestHeaders.GetValues("Test-Header").First());
    }

    [Fact]
    public void AddExternalClient_WithConfigurationBinding_ConfiguresHttpClientProperly()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "ConfigTestClient:BaseAddress", "http://config.com" },
            { "ConfigTestClient:Timeout", "00:00:05" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var services = new ServiceCollection();
        services.AddExternalClient<IConfigTestClient, ConfigTestClient>(configuration);
        var provider = services.BuildServiceProvider();

        // Act
        var client = provider.GetRequiredService<IConfigTestClient>() as ConfigTestClient;

        // Assert
        Assert.NotNull(client);
        Assert.Equal(new Uri("http://config.com"), client.HttpClient.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(5), client.HttpClient.Timeout);
    }

    [Fact]
    public void AddExternalClient_MultipleRegistrationsForSameClient_LastConfigurationOverrides()
    {
        // Arrange
        var services = new ServiceCollection();

        // First registration with initial settings
        services.AddExternalClient<IMultiRegistrationClient, MultiRegistrationClient>(opts =>
        {
            opts.BaseAddress = "http://first.com";
            opts.Timeout = TimeSpan.FromSeconds(5);
            opts.DefaultRequestHeaders = new Dictionary<string, string>
            {
                { "X-First", "Value1" }
            };
        });

        // Second registration that overrides some properties
        services.AddExternalClient<IMultiRegistrationClient, MultiRegistrationClient>(opts =>
        {
            // BaseAddress remains from the first registration.
            opts.Timeout = TimeSpan.FromSeconds(7);
            opts.DefaultRequestHeaders = new Dictionary<string, string>
            {
                { "X-Second", "Value2" }
            };
        });

        var provider = services.BuildServiceProvider();

        // Act
        var client = provider.GetRequiredService<IMultiRegistrationClient>() as MultiRegistrationClient;

        // Assert
        Assert.NotNull(client);
        // Expect the BaseAddress from the first registration.
        Assert.Equal(new Uri("http://first.com"), client.HttpClient.BaseAddress);
        // Expect the Timeout from the second registration.
        Assert.Equal(TimeSpan.FromSeconds(7), client.HttpClient.Timeout);
        // Only the header from the second registration should be applied.
        Assert.False(client.HttpClient.DefaultRequestHeaders.Contains("X-First"));
        Assert.True(client.HttpClient.DefaultRequestHeaders.Contains("X-Second"));
    }

    [Fact]
    public void AddExternalClient_DifferentClients_HaveIndependentConfigurations()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddExternalClient<IDiffClient1, DiffClient1>(opts =>
        {
            opts.BaseAddress = "http://client1.com";
            opts.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddExternalClient<IDiffClient2, DiffClient2>(opts =>
        {
            opts.BaseAddress = "http://client2.com";
            opts.Timeout = TimeSpan.FromSeconds(8);
        });

        var provider = services.BuildServiceProvider();

        // Act
        var client1 = provider.GetRequiredService<IDiffClient1>() as DiffClient1;
        var client2 = provider.GetRequiredService<IDiffClient2>() as DiffClient2;

        // Assert
        Assert.NotNull(client1);
        Assert.Equal(new Uri("http://client1.com"), client1.HttpClient.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(5), client1.HttpClient.Timeout);

        Assert.NotNull(client2);
        Assert.Equal(new Uri("http://client2.com"), client2.HttpClient.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(8), client2.HttpClient.Timeout);
    }

    [Fact]
    public void AddExternalClient_SameInterfaceDifferentImplementations_ResolvesAllClients()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddExternalClient<ICommonClient, CommonClientA>(opts =>
        {
            opts.BaseAddress = "http://clientA.com";
            opts.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddExternalClient<ICommonClient, CommonClientB>(opts =>
        {
            opts.BaseAddress = "http://clientB.com";
            opts.Timeout = TimeSpan.FromSeconds(8);
        });

        var provider = services.BuildServiceProvider();

        // Act
        var clients = provider.GetServices<ICommonClient>().ToList();

        // Assert
        Assert.Equal(2, clients.Count);
        var clientA = clients.OfType<CommonClientA>().FirstOrDefault();
        var clientB = clients.OfType<CommonClientB>().FirstOrDefault();

        Assert.NotNull(clientA);
        Assert.Equal(new Uri("http://clientA.com"), clientA.HttpClient.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(5), clientA.HttpClient.Timeout);

        Assert.NotNull(clientB);
        Assert.Equal(new Uri("http://clientB.com"), clientB.HttpClient.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(8), clientB.HttpClient.Timeout);
    }
}

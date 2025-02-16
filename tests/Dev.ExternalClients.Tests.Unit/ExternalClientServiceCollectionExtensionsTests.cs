using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Dev.ExternalClients.Tests.Unit;

public class ExternalClientServiceCollectionExtensionsTests
{
    public interface ITestClient
    {
        HttpClient HttpClient { get; }
    }

    public class TestClient(HttpClient httpClient) : ITestClient
    {
        public HttpClient HttpClient { get; } = httpClient;
    }

    public interface IAnotherTestClient
    {
        HttpClient HttpClient { get; }
    }

    public class AnotherTestClient(HttpClient httpClient) : IAnotherTestClient
    {
        public HttpClient HttpClient { get; } = httpClient;
    }

    [Fact]
    public void AddExternalClient_ShouldConfigureHttpClientWithOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddExternalClient<ITestClient, TestClient>(options =>
        {
            options.BaseAddress = "https://api.example.com";
            options.Timeout = TimeSpan.FromSeconds(5);
            options.DefaultRequestHeaders = new Dictionary<string, string>
            {
                    { "Authorization", "Bearer token" },
                    { "Accept", "application/json" }
            };
        });

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<ITestClient>();

        // Act
        var httpClient = client.HttpClient;

        // Assert
        Assert.Equal(new Uri("https://api.example.com"), httpClient.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(5), httpClient.Timeout);
        Assert.True(httpClient.DefaultRequestHeaders.Contains("Authorization"));
        Assert.True(httpClient.DefaultRequestHeaders.Contains("Accept"));
        Assert.Equal("Bearer token", httpClient.DefaultRequestHeaders.GetValues("Authorization").First());
        Assert.Equal("application/json", httpClient.DefaultRequestHeaders.GetValues("Accept").First());
    }

    [Fact]
    public void AddExternalClient_ShouldThrowExceptionForInvalidOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddExternalClient<ITestClient, TestClient>(options =>
        {
            options.BaseAddress = "invalid-url";
            options.Timeout = TimeSpan.FromSeconds(5);
        });

        // Act & Assert
        var serviceProvider = services.BuildServiceProvider();
        Assert.Throws<OptionsValidationException>(() => serviceProvider.GetRequiredService<ITestClient>());
    }

    [Fact]
    public void AddExternalClient_ShouldConfigureMultipleClientsIndependently()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddExternalClient<ITestClient, TestClient>(options =>
        {
            options.BaseAddress = "https://api.example.com";
            options.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddExternalClient<IAnotherTestClient, AnotherTestClient>(options =>
        {
            options.BaseAddress = "https://api.anotherexample.com";
            options.Timeout = TimeSpan.FromSeconds(10);
        });

        var serviceProvider = services.BuildServiceProvider();
        var testClient = serviceProvider.GetRequiredService<ITestClient>();
        var anotherTestClient = serviceProvider.GetRequiredService<IAnotherTestClient>();

        // Act
        var testHttpClient = testClient.HttpClient;
        var anotherTestHttpClient = anotherTestClient.HttpClient;

        // Assert
        Assert.Equal(new Uri("https://api.example.com"), testHttpClient.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(5), testHttpClient.Timeout);

        Assert.Equal(new Uri("https://api.anotherexample.com"), anotherTestHttpClient.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(10), anotherTestHttpClient.Timeout);
    }

    [Fact]
    public void AddExternalClient_ShouldApplyDefaultValues()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddExternalClient<ITestClient, TestClient>(options =>
        {
            options.BaseAddress = "https://api.example.com";
        });

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<ITestClient>();

        // Act
        var httpClient = client.HttpClient;

        // Assert
        Assert.Equal(new Uri("https://api.example.com"), httpClient.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(10), httpClient.Timeout); // Default timeout
    }
}

using Xunit;

namespace Dev.ExternalClients.Tests.Unit;

public class ExternalClientOptionsTests
{
    [Fact]
    public void ApplyTo_ShouldConfigureHttpClientCorrectly()
    {
        // Arrange
        var options = new ExternalClientOptions
        {
            BaseAddress = "https://api.example.com",
            Timeout = TimeSpan.FromSeconds(5),
            DefaultRequestHeaders = new Dictionary<string, string>
            {
                { "Authorization", "Bearer token" },
                { "Accept", "application/json" }
            }
        };

        using var httpClient = new HttpClient();

        // Act
        options.ApplyTo(httpClient);

        // Assert
        Assert.Equal(new Uri("https://api.example.com"), httpClient.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(5), httpClient.Timeout);
        Assert.True(httpClient.DefaultRequestHeaders.Contains("Authorization"));
        Assert.True(httpClient.DefaultRequestHeaders.Contains("Accept"));
        Assert.Equal("Bearer token", httpClient.DefaultRequestHeaders.GetValues("Authorization").First());
        Assert.Equal("application/json", httpClient.DefaultRequestHeaders.GetValues("Accept").First());
    }
}

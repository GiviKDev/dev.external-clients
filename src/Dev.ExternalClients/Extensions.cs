using Microsoft.Extensions.DependencyInjection;

namespace Dev.ExternalClients;

public static class Extensions
{
    public static IServiceCollection AddExternalClient<TClient, TImplementation>(
        this IServiceCollection services, Action<ExternalClientOptions> options)
        where TClient : class
        where TImplementation : class, TClient
    {
        var clientOptions = new ExternalClientOptions();
        options?.Invoke(clientOptions);

        services.AddHttpClient<TClient, TImplementation>(config =>
        {
            config.BaseAddress = new Uri(clientOptions.BaseAddress);
        });

        return services;
    }

    public static void Test(IServiceCollection services)
    {
        services.AddExternalClient<ITestClient, TestClient>(options =>
        {
            options.BaseAddress = "https://localhost:5001";
            options.Timeout = TimeSpan.FromSeconds(10);
        });
    }
}

public interface ITestClient { }
public class TestClient : ITestClient { }

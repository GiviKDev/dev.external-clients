using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dev.ExternalClients;

/// <summary>
/// Provides extension methods for adding and configuring external HTTP clients.
/// </summary>
public static class ExternalClientServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures an external HTTP client with the specified options.
    /// </summary>
    /// <typeparam name="TClient">The type of the client interface.</typeparam>
    /// <typeparam name="TImplementation">The type of the client implementation.</typeparam>
    /// <param name="services">The service collection to add the client to.</param>
    /// <param name="configureOptions">The action to configure the client options.</param>
    /// <returns>The service collection with the configured client added.</returns>
    public static IServiceCollection AddExternalClient<TClient, TImplementation>(
        this IServiceCollection services, Action<ExternalClientOptions> configureOptions)
        where TClient : class
        where TImplementation : class, TClient
    {
        services.AddOptions<ExternalClientOptions>(typeof(TImplementation).Name)
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<TClient, TImplementation>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<
                IOptionsSnapshot<ExternalClientOptions>>().Get(typeof(TImplementation).Name);

            options.ApplyTo(client);
        });

        return services;
    }
}

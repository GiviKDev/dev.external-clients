using Microsoft.Extensions.Configuration;
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
        var externalClientName = typeof(TImplementation).Name;

        services.AddExternalClientOptions(externalClientName)
            .Configure(configureOptions);

        services.AddExternalClient<TClient, TImplementation>(externalClientName);

        return services;
    }

    /// <summary>
    /// Adds and configures an external HTTP client using options from the configuration.
    /// </summary>
    /// <typeparam name="TClient">The type of the client interface.</typeparam>
    /// <typeparam name="TImplementation">The type of the client implementation.</typeparam>
    /// <param name="services">The service collection to add the client to.</param>
    /// <param name="configuration">The configuration instance to read the options from.</param>
    /// <returns>The service collection with the configured client added.</returns>
    public static IServiceCollection AddExternalClient<TClient, TImplementation>(
        this IServiceCollection services, IConfiguration configuration)
        where TClient : class
        where TImplementation : class, TClient
    {
        var externalClientName = typeof(TImplementation).Name;

        services.AddExternalClientOptions(externalClientName)
            .Bind(configuration.GetSection(externalClientName));

        services.AddExternalClient<TClient, TImplementation>(externalClientName);

        return services;
    }

    private static IHttpClientBuilder AddExternalClient<TClient, TImplementation>(
        this IServiceCollection services, string externalClientName)
        where TClient : class
        where TImplementation : class, TClient
    {
        var builder = services.AddHttpClient<TClient, TImplementation>(externalClientName, (serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<
                IOptionsSnapshot<ExternalClientOptions>>().Get(externalClientName);

            options.ApplyTo(client);
        });

        return builder;
    }

    private static OptionsBuilder<ExternalClientOptions> AddExternalClientOptions(
        this IServiceCollection services, string externalClientName)
    {
        var optionsBuilder = services.AddOptions<ExternalClientOptions>(externalClientName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return optionsBuilder;
    }
}

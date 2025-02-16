using System.ComponentModel.DataAnnotations;

namespace Dev.ExternalClients;

/// <summary>
/// Represents configuration options for an external HTTP client.
/// </summary>
public class ExternalClientOptions
{
    /// <summary>
    /// Gets or sets the base address of the external HTTP client.
    /// </summary>
    [Url]
    [Required]
    public string BaseAddress { get; set; } = null!;

    /// <summary>
    /// Gets or sets the request timeout for the HTTP client.
    /// </summary>
    [Range(typeof(TimeSpan), "00:00:00.100", "00:00:10")]
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gets or sets the default request headers to apply for every HTTP request.
    /// </summary>
    public IReadOnlyDictionary<string, string> DefaultRequestHeaders { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Applies these options to the specified <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="client">The HTTP client to configure.</param>
    public void ApplyTo(HttpClient client)
    {
        client.BaseAddress = new Uri(BaseAddress);
        client.Timeout = Timeout;

        foreach (var header in DefaultRequestHeaders)
        {
            client.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }
}

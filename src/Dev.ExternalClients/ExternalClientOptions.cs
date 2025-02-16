using System.ComponentModel.DataAnnotations;

namespace Dev.ExternalClients;

public class ExternalClientOptions
{
    [Url]
    [Required]
    public string BaseAddress { get; set; } = null!;

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}

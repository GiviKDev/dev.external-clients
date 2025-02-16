namespace Dev.ExternalClients.Tests.Unit;

// Helper types for the tests

// For action-based configuration test
public interface IActionTestClient
{
    HttpClient HttpClient { get; }
}
public class ActionTestClient(HttpClient client) : IActionTestClient
{
    public HttpClient HttpClient { get; } = client;
}

// For configuration-based binding test
public interface IConfigTestClient
{
    HttpClient HttpClient { get; }
}
public class ConfigTestClient(HttpClient client) : IConfigTestClient
{
    public HttpClient HttpClient { get; } = client;
}

// For multiple registrations test (same TClient/TImplementation)
public interface IMultiRegistrationClient
{
    HttpClient HttpClient { get; }
}
public class MultiRegistrationClient(HttpClient client) : IMultiRegistrationClient
{
    public HttpClient HttpClient { get; } = client;
}

// For different external clients test
public interface IDiffClient1
{
    HttpClient HttpClient { get; }
}
public class DiffClient1(HttpClient client) : IDiffClient1
{
    public HttpClient HttpClient { get; } = client;
}

public interface IDiffClient2
{
    HttpClient HttpClient { get; }
}
public class DiffClient2(HttpClient client) : IDiffClient2
{
    public HttpClient HttpClient { get; } = client;
}

// For same interface with different implementations test
public interface ICommonClient
{
    HttpClient HttpClient { get; }
}
public class CommonClientA(HttpClient client) : ICommonClient
{
    public HttpClient HttpClient { get; } = client;
}
public class CommonClientB(HttpClient client) : ICommonClient
{
    public HttpClient HttpClient { get; } = client;
}

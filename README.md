# external-clients

A .NET 9.0 library that provides configurable external HTTP client support. The library enables you to register and configure external HTTP clients via dependency injection (DI). You can configure clients either by providing an inline action or by binding to a section in your configuration (e.g., appsettings.json).

## Project Structure

- **src/Dev.ExternalClients/**  
  Contains the main library code:
  - **ExternalClientOptions.cs** – Defines configuration options for an external HTTP client, including properties for BaseAddress, Timeout, and DefaultRequestHeaders, and an `ApplyTo` method to configure an `HttpClient`.
  - **ExternalClientServiceCollectionExtensions.cs** – Provides extension methods to register external HTTP clients with DI. Supports inline configuration via an action and configuration binding from an `IConfiguration` instance.

- **tests/Dev.ExternalClients.Tests.Unit/**  
  Contains unit tests that:
  - Verify that `ExternalClientOptions` correctly configures an `HttpClient`.
  - Test various scenarios such as action-based configuration, configuration-based binding, multiple registrations, and different client setups.

- **Dev.ExternalClients.sln**  
  The Visual Studio solution file which includes the library and tests.

## Building the Project

Make sure you have the .NET 9.0 SDK installed on your Windows machine. From the repository root, run:

```sh
dotnet build
```

## Running the Tests

You can run unit tests by executing:

```sh
dotnet test
```

## Usage Examples

### 1. Inline Action Configuration

Register an external HTTP client with inline configuration in your application startup:

```csharp
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
```

### 2. Binding from appsettings.json

Provide configuration in your `appsettings.json` and bind to a client options instance:

**appsettings.json**

```json
{
  "TestClient": {
    "BaseAddress": "https://api.example.com",
    "Timeout": "00:00:05",
    "DefaultRequestHeaders": {
      "Authorization": "Bearer token",
      "Accept": "application/json"
    }
  }
}
```

**Program.cs / Startup.cs**

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dev.ExternalClients;

var builder = WebApplication.CreateBuilder(args);

// Bind the TestClient section from appsettings.json to ExternalClientOptions.
builder.Services.Configure<ExternalClientOptions>(
    builder.Configuration.GetSection("TestClient")
);

// Register the client using configuration binding.
builder.Services.AddExternalClient<ITestClient, TestClient>(
    builder.Configuration
);

var app = builder.Build();
app.Run();
```
# SDK Step 02: Connecting with Dependency Injection

This example shows how to integrate the Nuclia SDK with Microsoft's Dependency Injection (DI) container.

## Prerequisites

Before running this example, you need to set up your Nuclia credentials using .NET user secrets:

```bash
dotnet user-secrets set "ApiKey" "your-api-key-value"
dotnet user-secrets set "ZoneId" "your-zone-id-value"
dotnet user-secrets set "KnowledgeBoxId" "your-knowledge-box-id-value"
```

## Key Concepts

- Setting up Microsoft.Extensions.DependencyInjection
- Registering the NucliaDbClient with the DI container using `AddNucliaDb()`
- Resolving services from the service provider
- Using the `INucliaDbClient` interface for loose coupling

## Why This Matters

This approach is essential for:
- ASP.NET Core applications
- Blazor applications
- Any application following modern .NET patterns
- Testable and maintainable code architecture

## What This Example Does

1. Loads configuration from user secrets
2. Sets up a service collection
3. Registers the Nuclia SDK services using the `AddNucliaDb()` extension method
4. Builds the service provider
5. Resolves the `INucliaDbClient` from the DI container
6. Sends a question to your knowledge box
7. Displays the complete answer to the console

## Code Highlights

```csharp
// Create configuration
NucliaDbConfig config = new(
    ZoneId: userSecrets["ZoneId"],
    KnowledgeBoxId: userSecrets["KnowledgeBoxId"],
    ApiKey: userSecrets["ApiKey"]
);

// Set up DI
var services = new ServiceCollection();
services.AddNucliaDb(config);
var serviceProvider = services.BuildServiceProvider();

// Resolve client from DI
var client = serviceProvider.GetRequiredService<INucliaDbClient>();

// Use client
AskRequest askRequest = new("Create a question about the data in this knowledge box?");
var response = await client.Search.AskAsync(askRequest);
```

## Running This Example

1. Ensure your user secrets are configured (see Prerequisites above)

2. Navigate to this directory:
   ```bash
   cd samples/GettingStarted/Basics/SDK_Step02_Connecting_With_DI
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

## Next Steps

Once you understand DI integration, proceed to:
- **SDK_Step03_Streaming_Responses**: Learn how to stream responses in real-time

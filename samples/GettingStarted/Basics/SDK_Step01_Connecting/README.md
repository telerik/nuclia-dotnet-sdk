# SDK Step 01: Connecting to Nuclia

This example demonstrates the basic connection to the Nuclia API using the Progress Nuclia SDK.

## Prerequisites

Before running this example, you need to set up your Nuclia credentials using .NET user secrets:

```bash
dotnet user-secrets set "ApiKey" "your-api-key-value"
dotnet user-secrets set "ZoneId" "your-zone-id-value"
dotnet user-secrets set "KnowledgeBoxId" "your-knowledge-box-id-value"
```

## Key Concepts

- Installing and importing the Progress.Nuclia SDK
- Configuring credentials using `NucliaDbConfig`
- Creating a `NucliaDbClient` instance
- Making a simple request to the Ask API
- Receiving and displaying a complete response

## What This Example Does

1. Loads configuration from user secrets
2. Creates a `NucliaDbConfig` with your Nuclia credentials (Zone ID, Knowledge Box ID, API Key)
3. Instantiates a `NucliaDbClient` using the configuration
4. Sends a question to your knowledge box using the Ask API
5. Displays the complete answer to the console

## Code Highlights

```csharp
// Create configuration
NucliaDbConfig config = new(
    ZoneId: userSecrets["ZoneId"],
    KnowledgeBoxId: userSecrets["KnowledgeBoxId"],
    ApiKey: userSecrets["ApiKey"]
);

// Create client
using var client = new NucliaDbClient(config);

// Make request
AskRequest askRequest = new("Create a question about the data in this knowledge box?");
var response = await client.Search.AskAsync(askRequest);

// Display answer
Console.WriteLine(response.Data.Answer);
```

## Running This Example

1. Ensure your user secrets are configured (see Prerequisites above)

2. Navigate to this directory:
   ```bash
   cd samples/GettingStarted/Basics/SDK_Step01_Connecting
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

## Next Steps

Once you understand basic connectivity, proceed to:
- **SDK_Step02_Connecting_With_DI**: Learn how to integrate with Dependency Injection

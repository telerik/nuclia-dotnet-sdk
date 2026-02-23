// This is a simple example of how to connect to the Nuclia API using the Nuclia SDK. 
// Nullable has been disabled for simplicity
#nullable disable
using Microsoft.Extensions.Configuration;

// 1. Install the Nuclia SDK package
using Progress.Nuclia;
using Progress.Nuclia.Model;

// 2. Add your secrets
// dotnet user-secrets set "ApiKey" "your-api-key-value"
// dotnet user-secrets set "ZoneId" "your-zone-id-value"
// dotnet user-secrets set "KnowledgeBoxId" "your-knowledge-box-id-value"

// Build configuration with user secrets
var userSecrets = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// 3. Create a NucliaDbConfig instance with your credentials
NucliaDbConfig config = new(ZoneId: userSecrets["ZoneId"],
	KnowledgeBoxId: userSecrets["KnowledgeBoxId"],
	ApiKey: userSecrets["ApiKey"]);

// 4. Create a NucliaDbClient instance
using var client = new NucliaDbClient(config);

// 5. Test the connection by making a simple request to the Ask API
AskRequest askRequest = new("Create a question about the data in this knowledge box?");
var response = await client.Search.AskAsync(askRequest);

// 6. Write the answer to the console
Console.WriteLine("Answer:");
Console.WriteLine(response.Data.Answer);
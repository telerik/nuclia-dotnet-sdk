// This is a simple example of how to connect to the Nuclia API using the Nuclia SDK. 
// Nullable has been disabled for simplicity
#nullable disable
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// 1. Install the Nuclia SDK package
using Progress.Nuclia;
using Progress.Nuclia.Extensions;
using Progress.Nuclia.Model;
using Progress.Nuclia.Model.Streaming;

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

// 4. Set up Dependency Injection
var services = new ServiceCollection();

// 5. Register the NucliaDbClient with DI
services.AddNucliaDb(config);

// 6. Build the service provider
var serviceProvider = services.BuildServiceProvider();

// 7. Resolve the NucliaDbClient from DI
var client = serviceProvider.GetRequiredService<INucliaDbClient>();

// 8. Test the connection by making a simple request to the Ask API
AskRequest askRequest = new("Create a question about the data in this knowledge box?");

// 9. Stream the answer to the console
Console.WriteLine("Answer:");
await foreach (var chunk in client.Search.AskStreamAsync(askRequest))
{
	// Because streaming responses can include multiple types of content (answer, retrievals, citations),
	// we need to check the type of each chunk as it arrives and handle it accordingly.
	// In this example, we will only write the answer text to the console as it streams in.
	// Each answer chunk may contain a partial word or sub word,
	// so we will write them sequentially to build the full answer text in real time.
	if (chunk.Item is AnswerContent answer)
	{
		Console.Write(answer.Text);
	}
}
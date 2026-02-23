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

// 8. Stream the results to the console

// Add these variable declarations before the await foreach loop, near the top of your Main method or before usage
Dictionary<string, FindResource> originalResources = null;

Dictionary<string, int[][]> citations = null;

AskRequest askRequest = new("Write a question about this knowledge box?") { Citations = new Citations(true) };
await foreach (var chunk in client.Search.AskStreamAsync(askRequest))
{
	// Because streaming responses can include multiple types of content (answer, retrievals, citations),
	// we need to check the type of each chunk as it arrives and handle it accordingly.
	switch (chunk.Item)
	{
		case AnswerContent answer:
			// Each answer chunk may contain a partial word or sub word,
			// so we will write them sequentially to build the full answer text in real time.
			Console.Write(answer.Text);
			break;
		case RetrievalContent retrieval:
			originalResources = retrieval.Results?.Resources; 
			break;
		case CitationsContent c:
			citations = c.Citations;
			break;
	}
}

Console.WriteLine("\n\nCitations:");
// After the streaming response is complete, we can display the citations and their associated resources.
if (citations != null)
{
	foreach (var citation in citations)
	{
		Console.WriteLine($"Citation ID: {citation.Key}");
		Console.WriteLine("Associated Resources:");
		foreach (var resource in citation.Value)
		{
			Console.WriteLine($" - Resource ID: {resource[0]}, Score: {resource[1]}");
		}
	}
}

Console.WriteLine("\n\nResources:");
// We can also display the original resources that were retrieved, which may include additional metadata like titles, even for resources that were not ultimately cited in the answer.
if (originalResources != null)
{
	foreach (var resource in originalResources)
	{
		Console.WriteLine($"Resource ID: {resource.Key}, Title: {resource.Value.Title}");
	}
}
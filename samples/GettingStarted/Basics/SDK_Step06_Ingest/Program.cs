// This is a simple example of how to connect to the Nuclia API using the Nuclia SDK. 
// Nullable has been disabled for simplicity
#nullable disable
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// 1. Install the Nuclia SDK package
using Progress.Nuclia;
using Progress.Nuclia.Extensions;
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

// 4. Set up Dependency Injection
var services = new ServiceCollection();

// 5. Register the NucliaDbClient with DI
services.AddNucliaDb(config);

// 6. Build the service provider
var serviceProvider = services.BuildServiceProvider();

// 7. Resolve the NucliaDbClient from DI
var client = serviceProvider.GetRequiredService<INucliaDbClient>();

Console.ForegroundColor = ConsoleColor.Black;
Console.BackgroundColor = ConsoleColor.Yellow;
Console.WriteLine("Starting ingestion...");

try
{
	Console.WriteLine("Creating resource...");

	var createResponse = await client.Resources.CreateResourceAsync(new CreateResourcePayload
	{
		Title = "External Article",
		Summary = "Important article from external source",
		Links = new Dictionary<string, LinkField>
		{
			["article"] = new LinkField("https://edcharbeneau.com/")
			{
				Headers = new Dictionary<string, string>
				{
					["User-Agent"] = "ProgressARAG/1.0"
				}
			}
		}
	}
  );

	if (createResponse.Success)
	{
		var createdResource = createResponse.Data;
		Console.ForegroundColor = ConsoleColor.Black;
		Console.BackgroundColor = ConsoleColor.Green;
		Console.WriteLine("Resource created successfully");
		Console.ResetColor();
		Console.WriteLine($"New resource ID: {createdResource.Uuid}");
	}
	else
	{
		Console.ForegroundColor = ConsoleColor.Black;
		Console.BackgroundColor = ConsoleColor.Red;
		Console.WriteLine("Failed to create resource.");
		Console.ResetColor();
		Console.WriteLine($"Status Code: {createResponse.Error}, Error: {createResponse}");
	}
}
catch (Exception ex)
{
	Console.ForegroundColor = ConsoleColor.Black;
	Console.BackgroundColor = ConsoleColor.Red;
	Console.WriteLine("Error during ingestion process");
	Console.ResetColor();
	Console.WriteLine($"An error occurred: {ex.Message}");
}
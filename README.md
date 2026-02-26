# Progress.Nuclia .NET SDK

A comprehensive .NET SDK for Progress Agentic RAG's NucliaDb, providing RAG (Retrieval-Augmented Generation) capabilities with knowledge base management, AI-powered search, and resource operations.

## 📚 Documentation

- **[Full SDK Documentation](https://docs.rag.progress.cloud/docs/develop/dotnet-sdk/)** - Complete API reference, configuration options, and advanced features

## Installation

Install the NuGet package using the .NET CLI:

```bash
dotnet add package Progress.Nuclia
```

Or via Package Manager Console:

```powershell
Install-Package Progress.Nuclia
```

## Dependency Injection

Register the NucliaDb client in your dependency injection container with a fluent API.

### Basic Usage

```csharp
using Progress.Nuclia.Extensions;

// Create configuration
var config = new NucliaDbConfig(
    ZoneId: "aws-us-east-2-1",
    KnowledgeBoxId: "your-knowledge-box-id",
    ApiKey: "your-api-key"
);

// Create a client instance
using var nucliaDbClient = new NucliaDbClient(config);

// Or Register with DI and inject as nucliaDbClient
// builder.Services.AddNucliaDb(config).UseLogging();

// ask a question
var response = await nucliaDbClient.Search.AskAsync(new AskRequest("What is Nuclia?"));
Console.WriteLine(response.Data.Answer);
```

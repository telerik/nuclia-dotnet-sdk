# Progress.Nuclia .NET SDK

A comprehensive .NET SDK for Progress Agentic RAG's NucliaDb, providing RAG (Retrieval-Augmented Generation) capabilities with knowledge base management, AI-powered search, and resource operations.

## 📚 Documentation

- **[Full SDK Documentation](docs/index.md)** - Complete API reference, configuration options, and advanced features
- **[Getting Started Examples](samples/GettingStarted/Basics/)** - Step-by-step tutorials covering:
  - Basic connectivity
  - Dependency injection integration
  - Streaming responses
  - Citations and source attribution
  - Structured outputs
- **[Blazor Sample App](samples/GettingStarted/Blazor/SDK_Blazor_Ask/)** - Complete Blazor Web App demonstrating SDK integration

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

// Register with logging
builder.Services.AddNucliaDb(config).UseLogging();
```
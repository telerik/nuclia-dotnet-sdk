# Progress.Nuclia .NET SDK

A comprehensive .NET SDK for Progress Agentic RAG's NucliaDb, providing RAG (Retrieval-Augmented Generation) capabilities with knowledge base management, AI-powered search, and resource operations.

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

### With Configuration Factory

```csharp
builder.Services.AddNucliaDb(serviceProvider => 
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return new NucliaDbConfig(
        ZoneId: configuration["NucliaDb:ZoneId"]!,
        KnowledgeBoxId: configuration["NucliaDb:KnowledgeBoxId"]!,
        ApiKey: configuration["NucliaDb:ApiKey"]!
    );
}).UseLogging();
```

### With Custom HttpClient

```csharp
builder.Services.AddNucliaDb(config)
    .UseHttpClient(serviceProvider => 
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromMinutes(5);
        return httpClient;
    })
    .UseLogging();
```

### Without Logging

```csharp
// Register without logging (NullLoggerFactory will be used)
builder.Services.AddNucliaDb(config);
```
### Multiple NucliaDb Instances (Keyed Services)

For scenarios requiring multiple NucliaDb instances with different configurations (e.g., multi-tenant applications, multiple knowledge bases), use the keyed services registration:

```csharp
using NucliaDb.Extensions;

// Register multiple clients with different keys
var tenant1Config = new NucliaDbConfig(
    ZoneId: "aws-us-east-2-1",
    KnowledgeBoxId: "tenant1-kb-id",
    ApiKey: "tenant1-api-key"
);

var tenant2Config = new NucliaDbConfig(
    ZoneId: "aws-eu-central-1-1",
    KnowledgeBoxId: "tenant2-kb-id",
    ApiKey: "tenant2-api-key"
);

// Register each with a unique key
builder.Services.AddKeyedNucliaDb("tenant1", tenant1Config).UseLogging();
builder.Services.AddKeyedNucliaDb("tenant2", tenant2Config).UseLogging();

// Resolve specific instances by key
public class MultiTenantService
{
    private readonly NucliaDbClient _tenant1Client;
    private readonly NucliaDbClient _tenant2Client;
    
    public MultiTenantService(
        [FromKeyedServices("tenant1")] NucliaDbClient tenant1Client,
        [FromKeyedServices("tenant2")] NucliaDbClient tenant2Client)
    {
        _tenant1Client = tenant1Client;
        _tenant2Client = tenant2Client;
    }
    
    public async Task ProcessTenant1Data()
    {
        // Use tenant1's knowledge base
        var results = await _tenant1Client.Search.FindAsync(new FindRequest 
        { 
            Query = "search term" 
        });
    }
    
    public async Task ProcessTenant2Data()
    {
        // Use tenant2's knowledge base
        var results = await _tenant2Client.Search.FindAsync(new FindRequest 
        { 
            Query = "search term" 
        });
    }
}
```

#### Keyed Services with Configuration Factory

```csharp
// Register keyed services with dynamic configuration
builder.Services.AddKeyedNucliaDb("primary", sp => 
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new NucliaDbConfig(
        ZoneId: config["NucliaDb:Primary:ZoneId"]!,
        KnowledgeBoxId: config["NucliaDb:Primary:KnowledgeBoxId"]!,
        ApiKey: config["NucliaDb:Primary:ApiKey"]!
    );
}).UseLogging();

builder.Services.AddKeyedNucliaDb("secondary", sp => 
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new NucliaDbConfig(
        ZoneId: config["NucliaDb:Secondary:ZoneId"]!,
        KnowledgeBoxId: config["NucliaDb:Secondary:KnowledgeBoxId"]!,
        ApiKey: config["NucliaDb:Secondary:ApiKey"]!
    );
}).UseLogging();
```

#### Keyed Services with Custom HttpClient

```csharp
// Configure specific HttpClient settings per keyed instance
builder.Services.AddKeyedNucliaDb("longRunning", config)
    .UseHttpClient(sp => 
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromMinutes(30);
        return httpClient;
    })
    .UseLogging();
```

#### Benefits of Keyed Services

- **Multi-Tenancy**: Separate NucliaDb clients for different tenants or customers
- **Multiple Knowledge Bases**: Access different knowledge bases within the same application
- **Environment Separation**: Distinct configurations for development, staging, and production
- **Connection Pooling**: Uses `IHttpClientFactory` for efficient HTTP connection management
- **Isolated Logging**: Each keyed client maintains its own logging context

### Registration Options

- **Extension Methods**
  - `AddNucliaDb(NucliaDbConfig)` - Registers NucliaDbClient with the specified configuration
  - `AddNucliaDb(Func<IServiceProvider, NucliaDbConfig>)` - Registers NucliaDbClient with a configuration factory
  - `AddKeyedNucliaDb(object serviceKey, NucliaDbConfig)` - Registers a keyed NucliaDbClient with the specified configuration
  - `AddKeyedNucliaDb(object serviceKey, Func<IServiceProvider, NucliaDbConfig>)` - Registers a keyed NucliaDbClient with a configuration factory

- **Fluent Builder Methods**
  - `UseLogging()` - Enables logging using the registered `ILoggerFactory`
  - `UseHttpClient(Func<IServiceProvider, HttpClient>)` - Configures a custom `HttpClient` factory

- **Service Lifetime**
  - The `NucliaDbClient` is registered as a **Singleton** service, maintaining its own HttpClient and configuration throughout the application lifetime.

## SDK Services

The SDK provides three core services accessible through the `NucliaDbClient`:

### IKnowledgeBoxService

Manages knowledge base operations including:
- **Retrieve**: Knowledge box access by ID or slug
- **Configure**: Get, patch, and set knowledge base configurations
- **Monitor**: Statistics, counters, and debug information
- **Import/Export**: Data import/export operations with status tracking
- **Upload**: Simplified file upload for document ingestion
- **Schema**: Access to learning configuration JSON schemas

[?? View detailed IKnowledgeBoxService documentation](IKnowledgeBoxService.md)

### IResourceService

Handles individual resource (document) lifecycle management:
- **CRUD**: Create, read, update, and delete resource operations
- **Discovery**: Resource listing and pagination capabilities
- **Files**: Download field files and binary content access
- **Processing**: Reindex and reprocess resources for updates
- **Search**: Content search within individual resources
- **Metadata**: Resource properties and batch operations management

[?? View detailed IResourceService documentation](IResourceService.md)

### ISearchService

Provides AI-powered search and retrieval capabilities:
- **Ask**: Question-answering with RAG (synchronous and streaming)
- **Find**: Semantic paragraph-level search with detailed results
- **Catalog**: Resource browsing and listing
- **Search**: Traditional search across documents, paragraphs, and sentences
- **Suggest**: Query auto-suggestions with entity recognition
- **Summarization**: AI-powered content summarization
- **Graph Operations**: Knowledge graph search including nodes, relations, and paths

[?? View detailed ISearchService documentation](ISearchService.md)
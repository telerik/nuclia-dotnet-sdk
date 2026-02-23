# SDK Step 06: Ingesting External Content

This example demonstrates how to ingest external content into a Nuclia Knowledge Box by creating resources with links that will be automatically processed and indexed.

## Prerequisites

Before running this example, you need to set up your Nuclia credentials using .NET user secrets:

```bash
dotnet user-secrets set "ApiKey" "your-api-key-value"
dotnet user-secrets set "ZoneId" "your-zone-id-value"
dotnet user-secrets set "KnowledgeBoxId" "your-knowledge-box-id-value"
```

## Key Concepts

- Creating resources programmatically
- Ingesting external web content via links
- Setting custom HTTP headers for link processing
- Handling API responses with success/error checking
- Resource metadata (title, summary)
- Exception handling during ingestion

## Why This Matters

Content ingestion is fundamental to building a knowledge base:
- **Automated Content Collection**: Programmatically add content from external sources
- **Web Scraping**: Progress Agentic RAG automatically fetches and processes web content
- **Custom Headers**: Control how external content is accessed (authentication, user-agent, etc.)
- **Metadata Management**: Add titles and summaries for better organization
- **Batch Operations**: Foundation for bulk content ingestion workflows

## What This Example Does

1. Sets up the client using dependency injection
2. Creates a new resource with:
   - A descriptive title
   - A summary for context
   - A link to external content (a blog post URL)
   - Custom HTTP headers for the link request
3. Sends the resource creation request to the Knowledge Box
4. Handles both success and error responses
5. Displays color-coded console output for status
6. Shows the newly created resource ID on success

## Code Highlights

### Creating a Resource with External Link

```csharp
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
});
```

### Handling Response

```csharp
if (createResponse.Success)
{
    var createdResource = createResponse.Data;
    Console.WriteLine($"New resource ID: {createdResource.Uuid}");
}
else
{
    Console.WriteLine($"Status Code: {createResponse.Error}");
}
```

## Understanding Link Fields

### What Happens When You Add a Link

When you create a resource with a `LinkField`:
1. **Nuclia fetches the content** from the URL
2. **Content is extracted** (text, images, metadata)
3. **Processing occurs** (NLP, vectorization, indexing)
4. **Content becomes searchable** in your Knowledge Box

### Custom Headers

Headers allow you to:
- **Authenticate**: Add authorization tokens
- **Identify**: Set custom User-Agent strings
- **Configure**: Add any HTTP headers needed by the target server

```csharp
Headers = new Dictionary<string, string>
{
    ["User-Agent"] = "ProgressARAG/1.0",
    ["Authorization"] = "Bearer your-token",
    ["Accept-Language"] = "en-US"
}
```

## Resource Fields Explained

### Title
- **Purpose**: Human-readable name for the resource
- **Searchable**: Used in search results and citations
- **Best Practice**: Use descriptive, unique titles

### Summary
- **Purpose**: Brief description of the resource content
- **Context**: Helps with categorization and discovery
- **Optional**: Can be omitted if not needed

### Links Dictionary
- **Key**: Field identifier (e.g., "article", "documentation")
- **Value**: `LinkField` object with URL and optional headers
- **Multiple Links**: Can add multiple link fields to a single resource

## Running This Example

1. Ensure your user secrets are configured (see Prerequisites above)

2. Navigate to this directory:
   ```bash
   cd samples/GettingStarted/Basics/SDK_Step06_Ingest
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Observe the output:
   - Yellow: "Starting ingestion..."
   - Green: "Resource created successfully" + Resource ID
   - Red: Error messages (if any occur)

## What Happens After Ingestion

Once the resource is created:
1. **Processing Queue**: The link is added to Nuclia's processing queue
2. **Content Extraction**: The URL is fetched and content extracted
3. **Indexing**: Text is analyzed, vectorized, and indexed
4. **Searchable**: Content becomes available in search results
5. **Processing Time**: May take a few seconds to minutes depending on content size

Consider exploring:
- **File Upload**: Use `UploadKnowledgeBoxFileAsync` for local files (see [IKnowledgeBoxService](../../../docs/IKnowledgeBoxService.md))
- **Batch Operations**: Process multiple resources efficiently
- **Resource Updates**: Modify existing resources (see [IResourceService](../../../docs/IResourceService.md))
- **Field Management**: Add text, conversation, and file fields (see [IResourceFieldsService](../../../docs/IResourceFieldsService.md))
- **Monitor Processing**: Check resource processing status

## Related Documentation

- [IResourceService](../../../docs/IResourceService.md) - Resource CRUD operations
- [IResourceFieldsService](../../../docs/IResourceFieldsService.md) - Managing resource fields
- [IKnowledgeBoxService](../../../docs/IKnowledgeBoxService.md) - File uploads and imports

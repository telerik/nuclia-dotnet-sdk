# IResourceService Documentation

The `IResourceService` provides CRUD operations for resources within a Knowledge Box, including creation, retrieval, modification, deletion, and file management.

## Methods

### ListResourcesAsync

Retrieves a paginated list of resources from the Knowledge Box.

**Signature:**
```csharp
Task<ApiResponse<ResourceList>> ListResourcesAsync()
```

**Parameters:** None (all parameters are optional)

**Returns:** Paginated list of resources with metadata

**Example:**
```csharp
var response = await resourceService.ListResourcesAsync();
if (response.IsSuccessful)
{
    var resourceList = response.Data;
    Console.WriteLine($"Found {resourceList.Resources.Count} resources");
    foreach (var resource in resourceList.Resources)
    {
        Console.WriteLine($"Resource: {resource.Id} - {resource.Title}");
    }
}
```

---

### GetResourceByIdAsync

Retrieves a resource by its unique identifier.

**Signature:**
```csharp
Task<ApiResponse<NucliadbModelsResourceResource>> GetResourceByIdAsync(string resourceId)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource

**Returns:** Complete resource information including all fields and metadata

**Example:**
```csharp
var response = await resourceService.GetResourceByIdAsync("my-resource-id");
if (response.IsSuccessful)
{
    var resource = response.Data;
    Console.WriteLine($"Resource: {resource.Title}");
    Console.WriteLine($"Created: {resource.Created}");
}
```

---

### GetResourceBySlugAsync

Retrieves a resource by its slug identifier.

**Signature:**
```csharp
Task<ApiResponse<NucliadbModelsResourceResource>> GetResourceBySlugAsync(string slug)
```

**Parameters:**
- `slug` (string, required): The slug identifier of the resource

**Returns:** Complete resource information using human-readable slug

**Example:**
```csharp
var response = await resourceService.GetResourceBySlugAsync("my-document-slug");
if (response.IsSuccessful)
{
    var resource = response.Data;
    Console.WriteLine($"Found resource: {resource.Id}");
}
```

---

### DeleteResourceByIdAsync

Permanently deletes a resource by its unique identifier.

**Signature:**
```csharp
Task<ApiResponse<bool>> DeleteResourceByIdAsync(string resourceId)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource to delete

**Returns:** True if deletion was successful

**Example:**
```csharp
var response = await resourceService.DeleteResourceByIdAsync("my-resource-id");
if (response.IsSuccessful)
{
    Console.WriteLine("Resource deleted successfully");
}
```

---

### DeleteResourceBySlugAsync

Permanently deletes a resource by its slug identifier.

**Signature:**
```csharp
Task<ApiResponse<bool>> DeleteResourceBySlugAsync(string slug)
```

**Parameters:**
- `slug` (string, required): The slug identifier of the resource to delete

**Returns:** True if deletion was successful

**Example:**
```csharp
var response = await resourceService.DeleteResourceBySlugAsync("my-document-slug");
if (response.IsSuccessful)
{
    Console.WriteLine("Resource deleted successfully");
}
```

---

### DownloadFieldFileAsync (by Resource ID and Field ID)

Downloads field binary content by resource ID and field ID.

**Signature:**
```csharp
Task<ApiResponse<byte[]>> DownloadFieldFileAsync(string resourceId, string fieldId)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `fieldId` (string, required): The identifier of the field containing the file

**Returns:** Binary file content

**Example:**
```csharp
var response = await resourceService.DownloadFieldFileAsync("my-resource-id", "file-field-1");
if (response.IsSuccessful)
{
    await File.WriteAllBytesAsync("downloaded-file.pdf", response.Data);
    Console.WriteLine("File downloaded successfully");
}
```

---

### DownloadFieldFileAsync (by URL)

Downloads field binary content by complete resource URL path.

**Signature:**
```csharp
Task<ApiResponse<byte[]>> DownloadFieldFileAsync(string url)
```

**Parameters:**
- `url` (string, required): The complete URL path to the field file

**Returns:** Binary file content

**Example:**
```csharp
var url = "/kb/my-kb-id/resource/my-resource-id/file/file-field-1/download";
var response = await resourceService.DownloadFieldFileAsync(url);
if (response.IsSuccessful)
{
    await File.WriteAllBytesAsync("downloaded-file.pdf", response.Data);
    Console.WriteLine("File downloaded successfully");
}
```

---

### CreateResourceAsync

Creates a new resource in the Knowledge Box.

**Signature:**
```csharp
Task<ApiResponse<ResourceCreated>> CreateResourceAsync(CreateResourcePayload request)
```

**Parameters:**
- `request` (CreateResourcePayload, required): The resource data to create

**Returns:** Information about the created resource including its ID

**Example:**
```csharp
var resourceData = new CreateResourcePayload
{
    Title = "My New Document",
    Summary = "This is a sample document",
    Icon = "application/pdf"
};

var response = await resourceService.CreateResourceAsync(resourceData);
if (response.IsSuccessful)
{
    var created = response.Data;
    Console.WriteLine($"Resource created with ID: {created.Uuid}");
}
```

---

### ReindexResourceByIdAsync

Triggers reindexing of a resource by its ID (background operation).

**Signature:**
```csharp
Task<ApiResponse<bool>> ReindexResourceByIdAsync(string resourceId)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource to reindex

**Returns:** True if reindexing was triggered successfully

**Example:**
```csharp
var response = await resourceService.ReindexResourceByIdAsync("my-resource-id");
if (response.IsSuccessful)
{
    Console.WriteLine("Resource reindexing started");
}
```

---

### ReindexResourceBySlugAsync

Triggers reindexing of a resource by its slug (background operation).

**Signature:**
```csharp
Task<ApiResponse<bool>> ReindexResourceBySlugAsync(string slug)
```

**Parameters:**
- `slug` (string, required): The slug identifier of the resource to reindex

**Returns:** True if reindexing was triggered successfully

**Example:**
```csharp
var response = await resourceService.ReindexResourceBySlugAsync("my-document-slug");
if (response.IsSuccessful)
{
    Console.WriteLine("Resource reindexing started");
}
```

---

### ReprocessResourceByIdAsync

Triggers reprocessing of a resource by its ID.

**Signature:**
```csharp
Task<ApiResponse<ResourceUpdated>> ReprocessResourceByIdAsync(string resourceId)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource to reprocess

**Returns:** Resource update information (HTTP 202 Accepted)

**Example:**
```csharp
var response = await resourceService.ReprocessResourceByIdAsync("my-resource-id");
if (response.IsSuccessful)
{
    var updated = response.Data;
    Console.WriteLine($"Resource reprocessing started: {updated.SeqId}");
}
```

---

### ReprocessResourceBySlugAsync

Triggers reprocessing of a resource by its slug.

**Signature:**
```csharp
Task<ApiResponse<ResourceUpdated>> ReprocessResourceBySlugAsync(string slug)
```

**Parameters:**
- `slug` (string, required): The slug identifier of the resource to reprocess

**Returns:** Resource update information (HTTP 202 Accepted)

**Example:**
```csharp
var response = await resourceService.ReprocessResourceBySlugAsync("my-document-slug");
if (response.IsSuccessful)
{
    var updated = response.Data;
    Console.WriteLine($"Resource reprocessing started: {updated.SeqId}");
}
```

---

### ModifyResourceBySlugAsync

Partially updates a resource by its slug using PATCH operation.

**Signature:**
```csharp
Task<ApiResponse<ResourceUpdated>> ModifyResourceBySlugAsync(string slug, UpdateResourcePayload payload)
```

**Parameters:**
- `slug` (string, required): The slug identifier of the resource to modify
- `payload` (UpdateResourcePayload, required): The updates to apply

**Returns:** Resource update information

**Example:**
```csharp
var updates = new UpdateResourcePayload
{
    Title = "Updated Document Title",
    Summary = "Updated summary text"
};

var response = await resourceService.ModifyResourceBySlugAsync("my-document-slug", updates);
if (response.IsSuccessful)
{
    var updated = response.Data;
    Console.WriteLine($"Resource updated: {updated.SeqId}");
}
```

---

### ModifyResourceByIdAsync

Partially updates a resource by its ID using PATCH operation.

**Signature:**
```csharp
Task<ApiResponse<ResourceUpdated>> ModifyResourceByIdAsync(string resourceId, UpdateResourcePayload payload)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource to modify
- `payload` (UpdateResourcePayload, required): The updates to apply

**Returns:** Resource update information

**Example:**
```csharp
var updates = new UpdateResourcePayload
{
    Title = "Updated Document Title",
    Summary = "Updated summary text"
};

var response = await resourceService.ModifyResourceByIdAsync("my-resource-id", updates);
if (response.IsSuccessful)
{
    var updated = response.Data;
    Console.WriteLine($"Resource updated: {updated.SeqId}");
}
```

---

### RunResourceAgentsAsync

Executes processing agents for a resource by its ID.

**Signature:**
```csharp
Task<ApiResponse<bool>> RunResourceAgentsAsync(string resourceId, ResourceAgentsRequest request)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `request` (ResourceAgentsRequest, required): The agent execution configuration

**Returns:** True if agents were triggered successfully

**Example:**
```csharp
var agentsRequest = new ResourceAgentsRequest
{
    // Configure agents as needed
};

var response = await resourceService.RunResourceAgentsAsync("my-resource-id", agentsRequest);
if (response.IsSuccessful)
{
    Console.WriteLine("Resource agents execution started");
}
```

---

### SearchResourceAsync

Searches within a single resource by its ID.

**Signature:**
```csharp
Task<ApiResponse<KnowledgeboxSearchResults>> SearchResourceAsync(string resourceId, string query)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource to search
- `query` (string, required): The search query

**Returns:** Search results within the specified resource

**Example:**
```csharp
var response = await resourceService.SearchResourceAsync("my-resource-id", "important information");
if (response.IsSuccessful)
{
    var results = response.Data;
    Console.WriteLine($"Found {results.Total} matches in resource");
    foreach (var result in results.Resources)
    {
        Console.WriteLine($"Match: {result.Title}");
    }
}
```

## Usage Notes

- All methods are asynchronous and return `ApiResponse<T>` wrappers
- Check `response.IsSuccessful` before accessing `response.Data`
- File downloads return binary data that should be written to streams or files
- Reindex and reprocess operations are typically asynchronous background tasks
- PATCH operations allow partial updates without affecting unchanged fields
- Resource agents can be used for custom processing workflows
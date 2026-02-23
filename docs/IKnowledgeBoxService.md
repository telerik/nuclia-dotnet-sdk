# IKnowledgeBoxService Documentation

The `IKnowledgeBoxService` provides operations for managing Knowledge Boxes, including retrieval, configuration, export/import, and file uploads.

## Methods

### GetKnowledgeBoxAsync

Retrieves a Knowledge Box by its unique identifier.

**Signature:**
```csharp
Task<ApiResponse<KnowledgeBoxObj>> GetKnowledgeBoxAsync(string knowledgeBoxId)
```

**Parameters:**
- `knowledgeBoxId` (string, required): The unique identifier of the Knowledge Box

**Returns:** Complete Knowledge Box information including metadata

**Example:**
```csharp
var response = await knowledgeBoxService.GetKnowledgeBoxAsync("my-kb-id");
if (response.IsSuccessful)
{
    var knowledgeBox = response.Data;
    Console.WriteLine($"Knowledge Box: {knowledgeBox.Title}");
}
```

---

### GetKnowledgeBoxBySlugAsync

Retrieves a Knowledge Box by its slug identifier.

**Signature:**
```csharp
Task<ApiResponse<KnowledgeBoxObj>> GetKnowledgeBoxBySlugAsync(string slug)
```

**Parameters:**
- `slug` (string, required): The slug identifier of the Knowledge Box

**Returns:** Complete Knowledge Box information using human-readable slug

**Example:**
```csharp
var response = await knowledgeBoxService.GetKnowledgeBoxBySlugAsync("my-knowledge-base");
if (response.IsSuccessful)
{
    var knowledgeBox = response.Data;
    Console.WriteLine($"Found KB: {knowledgeBox.Id}");
}
```

---

### GetKnowledgeBoxCountersAsync

Gets counter summary for a Knowledge Box including resource counts and statistics.

**Signature:**
```csharp
Task<ApiResponse<KnowledgeboxCounters>> GetKnowledgeBoxCountersAsync(string knowledgeBoxId)
```

**Parameters:**
- `knowledgeBoxId` (string, required): The unique identifier of the Knowledge Box

**Returns:** Statistics including resource counts and processing information

**Example:**
```csharp
var response = await knowledgeBoxService.GetKnowledgeBoxCountersAsync("my-kb-id");
if (response.IsSuccessful)
{
    var counters = response.Data;
    Console.WriteLine($"Resources: {counters.Resources}");
}
```

---

### GetKnowledgeBoxConfigurationAsync

Retrieves the current models configuration of a Knowledge Box.

**Signature:**
```csharp
Task<ApiResponse<Dictionary<string, object>>> GetKnowledgeBoxConfigurationAsync(string knowledgeBoxId)
```

**Parameters:**
- `knowledgeBoxId` (string, required): The unique identifier of the Knowledge Box

**Returns:** Dynamic configuration object with various settings

**Example:**
```csharp
var response = await knowledgeBoxService.GetKnowledgeBoxConfigurationAsync("my-kb-id");
if (response.IsSuccessful)
{
    var config = response.Data;
    foreach (var setting in config)
    {
        Console.WriteLine($"{setting.Key}: {setting.Value}");
    }
}
```

---

### PatchKnowledgeBoxConfigurationAsync

Partially updates the models configuration of a Knowledge Box.

**Signature:**
```csharp
Task<ApiResponse<bool>> PatchKnowledgeBoxConfigurationAsync(string knowledgeBoxId, Dictionary<string, object> body)
```

**Parameters:**
- `knowledgeBoxId` (string, required): The unique identifier of the Knowledge Box
- `body` (Dictionary<string, object>, required): Configuration updates to apply

**Returns:** True if successful (HTTP 204)

**Example:**
```csharp
var updates = new Dictionary<string, object>
{
    ["learning_enabled"] = true,
    ["max_resources"] = 1000
};

var response = await knowledgeBoxService.PatchKnowledgeBoxConfigurationAsync("my-kb-id", updates);
if (response.IsSuccessful)
{
    Console.WriteLine("Configuration updated successfully");
}
```

---

### SetKnowledgeBoxConfigurationAsync

Replaces the entire models configuration of a Knowledge Box.

**Signature:**
```csharp
Task<ApiResponse<bool>> SetKnowledgeBoxConfigurationAsync(string knowledgeBoxId, Dictionary<string, object> body)
```

**Parameters:**
- `knowledgeBoxId` (string, required): The unique identifier of the Knowledge Box
- `body` (Dictionary<string, object>, required): Complete configuration to set

**Returns:** True if successful (HTTP 204)

**Example:**
```csharp
var fullConfig = new Dictionary<string, object>
{
    ["learning_enabled"] = true,
    ["semantic_model"] = "multilingual",
    ["anonymization_model"] = "disabled"
};

var response = await knowledgeBoxService.SetKnowledgeBoxConfigurationAsync("my-kb-id", fullConfig);
if (response.IsSuccessful)
{
    Console.WriteLine("Configuration replaced successfully");
}
```

---

### DownloadKnowledgeBoxExportAsync

Downloads an export for a Knowledge Box as binary data.

**Signature:**
```csharp
Task<ApiResponse<byte[]>> DownloadKnowledgeBoxExportAsync(string knowledgeBoxId, string exportId)
```

**Parameters:**
- `knowledgeBoxId` (string, required): The unique identifier of the Knowledge Box
- `exportId` (string, required): The identifier of the export to download

**Returns:** Binary data that can be saved to file or processed

**Example:**
```csharp
var response = await knowledgeBoxService.DownloadKnowledgeBoxExportAsync("my-kb-id", "export-123");
if (response.IsSuccessful)
{
    await File.WriteAllBytesAsync("kb-export.zip", response.Data);
    Console.WriteLine("Export downloaded successfully");
}
```

---

### GetKnowledgeBoxExportStatusAsync

Checks the status of a Knowledge Box export operation.

**Signature:**
```csharp
Task<ApiResponse<ExportStatus>> GetKnowledgeBoxExportStatusAsync(string knowledgeBoxId, string exportId)
```

**Parameters:**
- `knowledgeBoxId` (string, required): The unique identifier of the Knowledge Box
- `exportId` (string, required): The identifier of the export to check

**Returns:** Export progress and completion status

**Example:**
```csharp
var response = await knowledgeBoxService.GetKnowledgeBoxExportStatusAsync("my-kb-id", "export-123");
if (response.IsSuccessful)
{
    var status = response.Data;
    Console.WriteLine($"Export status: {status.Status}");
}
```

---

### GetKnowledgeBoxImportStatusAsync

Monitors the status of a Knowledge Box import operation.

**Signature:**
```csharp
Task<ApiResponse<ImportStatus>> GetKnowledgeBoxImportStatusAsync(string knowledgeBoxId, string importId)
```

**Parameters:**
- `knowledgeBoxId` (string, required): The unique identifier of the Knowledge Box
- `importId` (string, required): The identifier of the import to check

**Returns:** Import progress and completion status

**Example:**
```csharp
var response = await knowledgeBoxService.GetKnowledgeBoxImportStatusAsync("my-kb-id", "import-456");
if (response.IsSuccessful)
{
    var status = response.Data;
    Console.WriteLine($"Import status: {status.Status}");
}
```

---

### UploadKnowledgeBoxFileAsync

Uploads a file to a Knowledge Box (simplified single file upload).

**Signature:**
```csharp
Task<ApiResponse<ResourceFileUploaded>> UploadKnowledgeBoxFileAsync(string knowledgeBoxId, Stream fileStream, string fileName)
```

**Parameters:**
- `knowledgeBoxId` (string, required): The unique identifier of the Knowledge Box
- `fileStream` (Stream, required): The file content stream
- `fileName` (string, required): The name of the file

**Returns:** Information about the uploaded file

**Example:**
```csharp
using var fileStream = File.OpenRead("document.pdf");
var response = await knowledgeBoxService.UploadKnowledgeBoxFileAsync(
    "my-kb-id", 
    fileStream, 
    "document.pdf"
);

if (response.IsSuccessful)
{
    var uploadResult = response.Data;
    Console.WriteLine($"File uploaded: {uploadResult.FieldId}");
}
```

---

### GetLearningSchemaAsync

Retrieves the learning configuration JSON schema.

**Signature:**
```csharp
Task<ApiResponse<Dictionary<string, object>>> GetLearningSchemaAsync()
```

**Parameters:** None

**Returns:** JSON schema for validating learning configurations

**Example:**
```csharp
var response = await knowledgeBoxService.GetLearningSchemaAsync();
if (response.IsSuccessful)
{
    var schema = response.Data;
    Console.WriteLine($"Schema has {schema.Count} top-level properties");
}
```

## Usage Notes

- All methods are asynchronous and return `ApiResponse<T>` wrappers
- Check `response.IsSuccessful` before accessing `response.Data`
- File uploads use streaming for memory efficiency
- Export/import operations are asynchronous - use status methods to monitor progress
- Configuration methods support both partial (patch) and full (set) updates
# IResourceFieldsService Documentation

The `IResourceFieldsService` provides operations for managing individual fields within resources, including conversation, file, link, and text fields. It supports both resource ID and slug-based access patterns for all operations.

## Overview

Resource fields are the individual data components within a resource:
- **Conversation fields**: Store chat/conversation data with messages
- **File fields**: Store binary file data with metadata
- **Link fields**: Store web URLs and associated metadata
- **Text fields**: Store plain text or formatted text content

## Methods

### Conversation Field Operations

#### AddConversationFieldByIdAsync

Adds a new conversation field to a resource by ID.

**Signature:**
```csharp
Task<ApiResponse<ResourceFieldAdded>> AddConversationFieldByIdAsync(
    string resourceId,
    string fieldId,
    InputConversationField conversationField,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `fieldId` (string, required): The identifier for the new conversation field
- `conversationField` (InputConversationField, required): Conversation field data including messages
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** `ResourceFieldAdded` with sequence ID

**Example:**
```csharp
var conversationField = new InputConversationField
{
    Messages = new List<InputMessage>
    {
        new InputMessage(
            content: new InputMessageContent { Text = "Hello!" },
            ident: "msg1",
            who: "user"
        )
    }
};

var response = await client.ResourceFields.AddConversationFieldByIdAsync(
    "resource-123",
    "chat-field",
    conversationField
);

if (response.IsSuccessful)
{
    Console.WriteLine($"Conversation field added with seqid: {response.Data.Seqid}");
}
```

---

#### AddConversationFieldBySlugAsync

Adds a new conversation field to a resource by slug.

**Signature:**
```csharp
Task<ApiResponse<ResourceFieldAdded>> AddConversationFieldBySlugAsync(
    string resourceSlug,
    string fieldId,
    InputConversationField conversationField,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `resourceSlug` (string, required): The slug identifier of the resource
- `fieldId` (string, required): The identifier for the new conversation field
- `conversationField` (InputConversationField, required): Conversation field data
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** `ResourceFieldAdded` with sequence ID

---

#### AppendMessagesToConversationFieldByIdAsync

Appends additional messages to an existing conversation field.

**Signature:**
```csharp
Task<ApiResponse<object>> AppendMessagesToConversationFieldByIdAsync(
    string resourceId,
    string fieldId,
    List<InputMessage> messages,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `fieldId` (string, required): The identifier of the conversation field
- `messages` (List<InputMessage>, required): List of messages to append
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** Success response object

**Example:**
```csharp
var messages = new List<InputMessage>
{
    new InputMessage(
        content: new InputMessageContent { Text = "What is the weather?" },
        ident: "msg2",
        who: "user"
    ),
    new InputMessage(
        content: new InputMessageContent { Text = "It's sunny today!" },
        ident: "msg3",
        who: "assistant"
    )
};

var response = await client.ResourceFields.AppendMessagesToConversationFieldByIdAsync(
    "resource-123",
    "chat-field",
    messages
);
```

---

#### AppendMessagesToConversationFieldBySlugAsync

Appends additional messages to an existing conversation field by resource slug.

**Signature:**
```csharp
Task<ApiResponse<object>> AppendMessagesToConversationFieldBySlugAsync(
    string resourceSlug,
    string fieldId,
    List<InputMessage> messages,
    CancellationToken cancellationToken = default)
```

---

#### DownloadConversationAttachmentByIdAsync

Downloads a file attachment from a conversation message.

**Signature:**
```csharp
Task<ApiResponse<byte[]>> DownloadConversationAttachmentByIdAsync(
    string resourceId,
    string fieldId,
    string messageId,
    int fileNum,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `fieldId` (string, required): The identifier of the conversation field
- `messageId` (string, required): The identifier of the message containing the attachment
- `fileNum` (int, required): The index of the file attachment (0-based)
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** Binary file content as byte array

**Example:**
```csharp
var response = await client.ResourceFields.DownloadConversationAttachmentByIdAsync(
    "resource-123",
    "chat-field",
    "msg5",
    0
);

if (response.IsSuccessful)
{
    await File.WriteAllBytesAsync("attachment.pdf", response.Data);
}
```

---

#### DownloadConversationAttachmentBySlugAsync

Downloads a file attachment from a conversation message by resource slug.

**Signature:**
```csharp
Task<ApiResponse<byte[]>> DownloadConversationAttachmentBySlugAsync(
    string resourceSlug,
    string fieldId,
    string messageId,
    int fileNum,
    CancellationToken cancellationToken = default)
```

---

### File Field Operations

#### AddFileFieldByIdAsync

Adds a new file field to a resource by ID.

**Signature:**
```csharp
Task<ApiResponse<ResourceFieldAdded>> AddFileFieldByIdAsync(
    string resourceId,
    string fieldId,
    FileField fileField,
    bool skipStore = false,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `fieldId` (string, required): The identifier for the new file field
- `fileField` (FileField, required): File field data
- `skipStore` (bool, optional): If true, file will not be saved in blob storage (only sent to processing)
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** `ResourceFieldAdded` with sequence ID

**Example:**
```csharp
var fileField = new FileField(
    file: new File 
    { 
        Uri = "https://example.com/document.pdf" 
    }
)
{
    Language = "en",
    ExtractStrategy = "paragraph"
};

var response = await client.ResourceFields.AddFileFieldByIdAsync(
    "resource-123",
    "document-field",
    fileField,
    skipStore: false
);
```

---

#### AddFileFieldBySlugAsync

Adds a new file field to a resource by slug.

**Signature:**
```csharp
Task<ApiResponse<ResourceFieldAdded>> AddFileFieldBySlugAsync(
    string resourceSlug,
    string fieldId,
    FileField fileField,
    bool skipStore = false,
    CancellationToken cancellationToken = default)
```

---

#### UploadFileByIdAsync

Uploads a binary file directly to a resource field by ID.

**Signature:**
```csharp
Task<ApiResponse<ResourceFileUploaded>> UploadFileByIdAsync(
    string resourceId,
    string fieldId,
    byte[] fileContent,
    string? filename = null,
    string? password = null,
    string? language = null,
    string? md5 = null,
    string? extractStrategy = null,
    string? splitStrategy = null,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `fieldId` (string, required): The identifier for the file field
- `fileContent` (byte[], required): Binary content of the file
- `filename` (string, optional): Name of the file being uploaded
- `password` (string, optional): Password if the file is password protected
- `language` (string, optional): Language code of the file content
- `md5` (string, optional): MD5 hash for file deduplication
- `extractStrategy` (string, optional): Strategy to use for content extraction
- `splitStrategy` (string, optional): Strategy to use for content splitting
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** `ResourceFileUploaded` with upload details

**Example:**
```csharp
var fileBytes = await File.ReadAllBytesAsync("document.pdf");

var response = await client.ResourceFields.UploadFileByIdAsync(
    resourceId: "resource-123",
    fieldId: "pdf-document",
    fileContent: fileBytes,
    filename: "document.pdf",
    language: "en",
    extractStrategy: "paragraph"
);

if (response.IsSuccessful)
{
    Console.WriteLine($"File uploaded: {response.Data.FieldId}");
    Console.WriteLine($"UUID: {response.Data.Uuid}");
}
```

---

#### UploadFileBySlugAsync

Uploads a binary file directly to a resource field by slug.

**Signature:**
```csharp
Task<ApiResponse<ResourceFileUploaded>> UploadFileBySlugAsync(
    string resourceSlug,
    string fieldId,
    byte[] fileContent,
    string? filename = null,
    string? password = null,
    string? language = null,
    string? md5 = null,
    string? extractStrategy = null,
    string? splitStrategy = null,
    CancellationToken cancellationToken = default)
```

---

#### DownloadFileFieldByIdAsync

Downloads the binary content of a file field by resource ID.

**Signature:**
```csharp
Task<ApiResponse<byte[]>> DownloadFileFieldByIdAsync(
    string resourceId,
    string fieldId,
    bool inline = false,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `fieldId` (string, required): The identifier of the file field
- `inline` (bool, optional): Whether to display the file inline (affects Content-Disposition header)
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** Binary file content as byte array

**Example:**
```csharp
var response = await client.ResourceFields.DownloadFileFieldByIdAsync(
    "resource-123",
    "pdf-document"
);

if (response.IsSuccessful)
{
    await File.WriteAllBytesAsync("downloaded.pdf", response.Data);
}
```

---

#### DownloadFileFieldBySlugAsync

Downloads the binary content of a file field by resource slug.

**Signature:**
```csharp
Task<ApiResponse<byte[]>> DownloadFileFieldBySlugAsync(
    string resourceSlug,
    string fieldId,
    bool inline = false,
    CancellationToken cancellationToken = default)
```

---

#### ReprocessFileFieldByIdAsync

Triggers reprocessing of a file field by resource ID.

**Signature:**
```csharp
Task<ApiResponse<bool>> ReprocessFileFieldByIdAsync(
    string resourceId,
    string fieldId,
    bool? resetTitle = null,
    string? filePassword = null,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `fieldId` (string, required): The identifier of the file field
- `resetTitle` (bool, optional): Whether to reset the resource title from the file
- `filePassword` (string, optional): Password for the file if it's password protected
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** True if reprocessing was triggered successfully

**Example:**
```csharp
var response = await client.ResourceFields.ReprocessFileFieldByIdAsync(
    "resource-123",
    "pdf-document",
    resetTitle: true,
    filePassword: "secret123"
);
```

---

#### ReprocessFileFieldBySlugAsync

Triggers reprocessing of a file field by resource slug.

**Signature:**
```csharp
Task<ApiResponse<bool>> ReprocessFileFieldBySlugAsync(
    string resourceSlug,
    string fieldId,
    bool? resetTitle = null,
    string? filePassword = null,
    CancellationToken cancellationToken = default)
```

---

### Link Field Operations

#### AddLinkFieldByIdAsync

Adds a new link field to a resource by ID.

**Signature:**
```csharp
Task<ApiResponse<ResourceFieldAdded>> AddLinkFieldByIdAsync(
    string resourceId,
    string fieldId,
    LinkField linkField,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `fieldId` (string, required): The identifier for the new link field
- `linkField` (LinkField, required): Link field data including URI
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** `ResourceFieldAdded` with sequence ID

**Example:**
```csharp
var linkField = new LinkField("https://example.com/article")
{
    Headers = new Dictionary<string, string>
    {
        ["User-Agent"] = "MyBot/1.0"
    },
    Language = "en",
    CssSelector = "article.content"
};

var response = await client.ResourceFields.AddLinkFieldByIdAsync(
    "resource-123",
    "web-link",
    linkField
);
```

---

#### AddLinkFieldBySlugAsync

Adds a new link field to a resource by slug.

**Signature:**
```csharp
Task<ApiResponse<ResourceFieldAdded>> AddLinkFieldBySlugAsync(
    string resourceSlug,
    string fieldId,
    LinkField linkField,
    CancellationToken cancellationToken = default)
```

---

### Text Field Operations

#### AddTextFieldByIdAsync

Adds a new text field to a resource by ID.

**Signature:**
```csharp
Task<ApiResponse<ResourceFieldAdded>> AddTextFieldByIdAsync(
    string resourceId,
    string fieldId,
    TextField textField,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `fieldId` (string, required): The identifier for the new text field
- `textField` (TextField, required): Text field data including body content
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** `ResourceFieldAdded` with sequence ID

**Example:**
```csharp
var textField = new TextField("This is the text content of my document.")
{
    Format = TextFormat.Plain,
    ExtractStrategy = "paragraph"
};

var response = await client.ResourceFields.AddTextFieldByIdAsync(
    "resource-123",
    "content-field",
    textField
);
```

---

#### AddTextFieldBySlugAsync

Adds a new text field to a resource by slug.

**Signature:**
```csharp
Task<ApiResponse<ResourceFieldAdded>> AddTextFieldBySlugAsync(
    string resourceSlug,
    string fieldId,
    TextField textField,
    CancellationToken cancellationToken = default)
```

---

### Generic Field Operations

#### GetResourceFieldByIdAsync

Retrieves a resource field by resource ID and field type.

**Signature:**
```csharp
Task<ApiResponse<ResourceField>> GetResourceFieldByIdAsync(
    string resourceId,
    FieldTypeName fieldType,
    string fieldId,
    ResourceFieldProperties[]? show = null,
    ExtractedDataTypeName[]? extracted = null,
    string? page = null,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `fieldType` (FieldTypeName, required): The type of field (Text, File, Link, Conversation, Generic)
- `fieldId` (string, required): The identifier of the field
- `show` (ResourceFieldProperties[], optional): Properties to include (Value, Extracted, Error)
- `extracted` (ExtractedDataTypeName[], optional): Extracted data types to include
- `page` (string, optional): Page number or 'last' for conversation fields
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** `ResourceField` with field data and metadata

**Example:**
```csharp
var response = await client.ResourceFields.GetResourceFieldByIdAsync(
    resourceId: "resource-123",
    fieldType: FieldTypeName.Text,
    fieldId: "content-field",
    show: new[] { ResourceFieldProperties.Value, ResourceFieldProperties.Extracted }
);

if (response.IsSuccessful)
{
    Console.WriteLine($"Field ID: {response.Data.FieldId}");
    Console.WriteLine($"Field Type: {response.Data.FieldType}");
}
```

---

#### GetResourceFieldBySlugAsync

Retrieves a resource field by resource slug and field type.

**Signature:**
```csharp
Task<ApiResponse<ResourceField>> GetResourceFieldBySlugAsync(
    string resourceSlug,
    FieldTypeName fieldType,
    string fieldId,
    ResourceFieldProperties[]? show = null,
    ExtractedDataTypeName[]? extracted = null,
    string? page = null,
    CancellationToken cancellationToken = default)
```

---

#### DeleteResourceFieldByIdAsync

Deletes a resource field by resource ID.

**Signature:**
```csharp
Task<ApiResponse<bool>> DeleteResourceFieldByIdAsync(
    string resourceId,
    FieldTypeName fieldType,
    string fieldId,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `fieldType` (FieldTypeName, required): The type of field to delete
- `fieldId` (string, required): The identifier of the field to delete
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** True if deletion was successful

**Example:**
```csharp
var response = await client.ResourceFields.DeleteResourceFieldByIdAsync(
    "resource-123",
    FieldTypeName.Text,
    "old-content-field"
);

if (response.IsSuccessful)
{
    Console.WriteLine("Field deleted successfully");
}
```

---

#### DeleteResourceFieldBySlugAsync

Deletes a resource field by resource slug.

**Signature:**
```csharp
Task<ApiResponse<bool>> DeleteResourceFieldBySlugAsync(
    string resourceSlug,
    FieldTypeName fieldType,
    string fieldId,
    CancellationToken cancellationToken = default)
```

---

#### DownloadExtractedFileByIdAsync

Downloads an extracted binary file from a field by resource ID.

**Signature:**
```csharp
Task<ApiResponse<byte[]>> DownloadExtractedFileByIdAsync(
    string resourceId,
    FieldTypeName fieldType,
    string fieldId,
    string downloadField,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `fieldType` (FieldTypeName, required): The type of field
- `fieldId` (string, required): The identifier of the field
- `downloadField` (string, required): The identifier of the extracted file to download
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** Binary file content as byte array

**Example:**
```csharp
var response = await client.ResourceFields.DownloadExtractedFileByIdAsync(
    resourceId: "resource-123",
    fieldType: FieldTypeName.File,
    fieldId: "pdf-document",
    downloadField: "thumbnail"
);

if (response.IsSuccessful)
{
    await File.WriteAllBytesAsync("thumbnail.jpg", response.Data);
}
```

---

#### DownloadExtractedFileBySlugAsync

Downloads an extracted binary file from a field by resource slug.

**Signature:**
```csharp
Task<ApiResponse<byte[]>> DownloadExtractedFileBySlugAsync(
    string resourceSlug,
    FieldTypeName fieldType,
    string fieldId,
    string downloadField,
    CancellationToken cancellationToken = default)
```

---

## Common Patterns

### Working with Field IDs

Field IDs must match the pattern `^[a-zA-Z0-9:_-]+$`:

```csharp
// Valid field IDs
"my-field"
"field_1"
"namespace:field"
"field-with-underscore_123"

// Invalid field IDs (will cause validation errors)
"field with spaces"
"field@special"
```

### Error Handling

All methods return `ApiResponse<T>` which includes error handling:

```csharp
var response = await client.ResourceFields.AddTextFieldByIdAsync(
    "resource-123",
    "content",
    textField
);

if (response.IsSuccessful)
{
    // Success
    var result = response.Data;
}
else if (response.ValidationError != null)
{
    // Validation error (HTTP 422)
    Console.WriteLine($"Validation failed: {response.ValidationError}");
}
else
{
    // Other error
    Console.WriteLine($"Error: {response.ErrorMessage}");
}
```

### Uploading vs Adding File Fields

There are two ways to add file content:

1. **Direct Upload** (recommended for binary data you have in memory):
```csharp
var fileBytes = await File.ReadAllBytesAsync("document.pdf");
var response = await client.ResourceFields.UploadFileByIdAsync(
    "resource-123",
    "doc",
    fileBytes,
    filename: "document.pdf"
);
```

2. **File Field with URI** (for files accessible via URL):
```csharp
var fileField = new FileField(new File { Uri = "https://example.com/doc.pdf" });
var response = await client.ResourceFields.AddFileFieldByIdAsync(
    "resource-123",
    "doc",
    fileField
);
```

### Skip Store Option

The `skipStore` parameter allows you to send files for processing without storing them in blob storage:

```csharp
// File will be processed but not stored
var response = await client.ResourceFields.AddFileFieldByIdAsync(
    "resource-123",
    "temp-file",
    fileField,
    skipStore: true  // File is processed but not permanently stored
);
```

This is useful for temporary processing or when you only need the extracted data.

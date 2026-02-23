# ISearchService Documentation

The `ISearchService` provides AI-powered search, question-answering, and retrieval capabilities across Knowledge Boxes, including both synchronous and streaming responses.

## Methods

### AskAsync

Ask a question to the Knowledge Box and get a synchronous response with RAG (Retrieval-Augmented Generation).

**Signature:**
```csharp
Task<ApiResponse<SyncAskResponse>> AskAsync(AskRequest request)
```

**Parameters:**
- `request` (AskRequest, required): The ask request containing query and configuration

**Returns:** Complete response with answer, citations, and metadata

**Example:**
```csharp
var askRequest = new AskRequest
{
    Query = "What are the main benefits of using this system?"
};

var response = await searchService.AskAsync(askRequest);
if (response.IsSuccessful)
{
    var result = response.Data;
    Console.WriteLine($"Answer: {result.Answer}");
    Console.WriteLine($"Citations: {result.Citations?.Count ?? 0}");
}
```

---

### AskStreamAsync (with AskRequest)

Ask a question to the Knowledge Box and get a streaming response.

**Signature:**
```csharp
IAsyncEnumerable<SyncAskResponseUpdate> AskStreamAsync(AskRequest request)
```

**Parameters:**
- `request` (AskRequest, required): The ask request containing query and configuration

**Returns:** Streaming updates as they become available

**Example:**
```csharp
var askRequest = new AskRequest(query) { Citations = new Citations(true) };

var result = client.Search.AskStreamAsync(askRequest);
string streamingResponse = string.Empty;
Dictionary<string, FindResource>? allResources;
IEnumerable<(int Number, KeyValuePair<string, int[][]> Item)>? citations;
await foreach (var response in result)
 	{
 		switch (response.Item)
 		{
 			case AnswerContent answer:
 				streamingResponse += answer.Text;
 				break;
 			case RetrievalContent retrieval:
 				allResources = retrieval.Results?.Resources;
 				break;
 			case CitationsContent c:
 				citations = c.Citations.Index(); // Index is optional, this adds an numeric index to the citations.
 				break;
 		}
 	}
```

---

### AskStreamAsync (with query string)

Ask a question using default options and get a streaming response.

**Signature:**
```csharp
IAsyncEnumerable<SyncAskResponseUpdate> AskStreamAsync(string query)
```

**Parameters:**
- `query` (string, required): The question to ask

**Returns:** Streaming response updates

**Example:**
```csharp
string streamingResponse = string.Empty;
await foreach (var update in searchService.AskStreamAsync("How do I configure authentication?"))
{
    if (update.Item is AnswerContent answerContent)
    {
       streamingResponse += answerContent.Text;
    }
}
```

---

### FindAsync

Find resources in the Knowledge Box with detailed paragraph-level search results.

**Signature:**
```csharp
Task<ApiResponse<KnowledgeboxFindResults>> FindAsync(FindRequest request)
```

**Parameters:**
- `request` (FindRequest, required): The find request with search parameters

**Returns:** Detailed search results with paragraph-level information

**Example:**
```csharp
var findRequest = new FindRequest
{
    Query = "security implementation"
};

var response = await searchService.FindAsync(findRequest);
if (response.IsSuccessful)
{
    var results = response.Data;
    Console.WriteLine($"Found {results.Total} results");
    foreach (var resource in results.Resources)
    {
        Console.WriteLine($"Resource: {resource.Title}");
    }
}
```

---

### CatalogAsync

List resources in the Knowledge Box using POST with JSON body.

**Signature:**
```csharp
Task<ApiResponse<KnowledgeboxSearchResults>> CatalogAsync(CatalogRequest request)
```

**Parameters:**
- `request` (CatalogRequest, required): The catalog request parameters

**Returns:** Resource catalog with filtering and pagination

**Example:**
```csharp
var catalogRequest = new CatalogRequest
{
    // Configure catalog parameters
};

var response = await searchService.CatalogAsync(catalogRequest);
if (response.IsSuccessful)
{
    var catalog = response.Data;
    Console.WriteLine($"Catalog contains {catalog.Total} resources");
}
```

---

### SuggestAsync

Retrieve paragraph and entity suggestions for a query.

**Signature:**
```csharp
Task<ApiResponse<KnowledgeboxSuggestResults>> SuggestAsync(string query)
```

**Parameters:**
- `query` (string, required): The query to get suggestions for

**Returns:** Suggestions including paragraphs and entities

**Example:**
```csharp
var response = await searchService.SuggestAsync("authentication");
if (response.IsSuccessful)
{
    var suggestions = response.Data;
    Console.WriteLine($"Found {suggestions.Paragraphs?.Count ?? 0} paragraph suggestions");
    Console.WriteLine($"Found {suggestions.Entities?.Count ?? 0} entity suggestions");
}
```

---

### SummarizeAsync

Summarize a set of resources using AI.

**Signature:**
```csharp
Task<ApiResponse<SummarizedResponse>> SummarizeAsync(SummarizeRequest request)
```

**Parameters:**
- `request` (SummarizeRequest, required): The summarization request with resources and options

**Returns:** AI-generated summary of the specified resources

**Example:**
```csharp
var summarizeRequest = new SummarizeRequest
{
    Resources = new[] { "resource-1", "resource-2" }
};

var response = await searchService.SummarizeAsync(summarizeRequest);
if (response.IsSuccessful)
{
    var summary = response.Data;
    Console.WriteLine($"Summary: {summary.Summary}");
}
```

---

### GraphSearchAsync

Search Knowledge Box graph using triplets (vertex-edge-vertex).

**Signature:**
```csharp
Task<ApiResponse<GraphSearchResponse>> GraphSearchAsync(GraphSearchRequest request)
```

**Parameters:**
- `request` (GraphSearchRequest, required): Graph search parameters

**Returns:** Graph search results with relationships

**Example:**
```csharp
var graphRequest = new GraphSearchRequest
{
    Query = "security protocols"
};

var response = await searchService.GraphSearchAsync(graphRequest);
if (response.IsSuccessful)
{
    var results = response.Data;
    Console.WriteLine($"Found {results.Paths?.Count ?? 0} graph paths");
}
```

---

### GraphNodesSearchAsync

Search Knowledge Box graph nodes (vertices).

**Signature:**
```csharp
Task<ApiResponse<GraphNodesSearchResponse>> GraphNodesSearchAsync(GraphNodesSearchRequest request)
```

**Parameters:**
- `request` (GraphNodesSearchRequest, required): Graph nodes search parameters

**Returns:** Graph nodes matching the search criteria

**Example:**
```csharp
var nodesRequest = new GraphNodesSearchRequest
{
    Query = "user authentication"
};

var response = await searchService.GraphNodesSearchAsync(nodesRequest);
if (response.IsSuccessful)
{
    var nodes = response.Data;
    Console.WriteLine($"Found {nodes.Nodes?.Count ?? 0} matching nodes");
}
```

---

### GraphRelationsSearchAsync

Search Knowledge Box graph relations (edges).

**Signature:**
```csharp
Task<ApiResponse<GraphRelationsSearchResponse>> GraphRelationsSearchAsync(GraphRelationsSearchRequest request)
```

**Parameters:**
- `request` (GraphRelationsSearchRequest, required): Graph relations search parameters

**Returns:** Graph relations matching the search criteria

**Example:**
```csharp
var relationsRequest = new GraphRelationsSearchRequest
{
    Query = "implements"
};

var response = await searchService.GraphRelationsSearchAsync(relationsRequest);
if (response.IsSuccessful)
{
    var relations = response.Data;
    Console.WriteLine($"Found {relations.Relations?.Count ?? 0} matching relations");
}
```

---

### SendFeedbackAsync

Send feedback for a search operation to improve future results.

**Signature:**
```csharp
Task<ApiResponse<object>> SendFeedbackAsync(FeedbackRequest request)
```

**Parameters:**
- `request` (FeedbackRequest, required): Feedback data including operation ID and rating

**Returns:** True if feedback was submitted successfully

**Example:**
```csharp
var feedbackRequest = new FeedbackRequest
{
    Ident = "operation-123",
    Good = true,
    Task = TaskType.Question
};

var response = await searchService.SendFeedbackAsync(feedbackRequest);
if (response.IsSuccessful)
{
    Console.WriteLine("Feedback submitted successfully");
}
```

---

### SearchAsync

Search Knowledge Box and retrieve separate results for documents, paragraphs, and sentences.

**Signature:**
```csharp
Task<ApiResponse<KnowledgeboxSearchResults>> SearchAsync(SearchRequest request)
```

**Parameters:**
- `request` (SearchRequest, required): Comprehensive search request parameters

**Returns:** Structured search results with multiple result types

**Example:**
```csharp
var searchRequest = new SearchRequest
{
    Query = "API documentation"
};

var response = await searchService.SearchAsync(searchRequest);
if (response.IsSuccessful)
{
    var results = response.Data;
    Console.WriteLine($"Documents: {results.Documents?.Count ?? 0}");
    Console.WriteLine($"Paragraphs: {results.Paragraphs?.Count ?? 0}");
    Console.WriteLine($"Sentences: {results.Sentences?.Count ?? 0}");
}
```

---

### AskResourceAsync

Ask a question to a specific resource and get a synchronous response.

**Signature:**
```csharp
Task<ApiResponse<SyncAskResponse>> AskResourceAsync(string resourceId, AskRequest request)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `request` (AskRequest, required): The ask request parameters

**Returns:** Answer specific to the resource

**Example:**
```csharp
var askRequest = new AskRequest
{
    Query = "What does this document explain?"
};

var response = await searchService.AskResourceAsync("my-resource-id", askRequest);
if (response.IsSuccessful)
{
    var result = response.Data;
    Console.WriteLine($"Resource-specific answer: {result.Answer}");
}
```

---

### AskResourceStreamAsync (with AskRequest)

Ask a question to a specific resource and get a streaming response.

**Signature:**
```csharp
IAsyncEnumerable<SyncAskResponseUpdate> AskResourceStreamAsync(string resourceId, AskRequest request)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `request` (AskRequest, required): The ask request parameters

**Returns:** Streaming response specific to the resource

**Example:**
```csharp
var askRequest = new AskRequest
{
    Query = "Summarize this document's main points"
};

await foreach (var update in searchService.AskResourceStreamAsync("my-resource-id", askRequest))
{
    if (update.Item is AnswerContent answerContent && !string.IsNullOrEmpty(answerContent.Text))
    {
        Console.Write(answerContent.Text);
    }
}
```

---

### AskResourceStreamAsync (with query string)

Ask a question to a specific resource with simplified query.

**Signature:**
```csharp
IAsyncEnumerable<SyncAskResponseUpdate> AskResourceStreamAsync(string resourceId, string query)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `query` (string, required): The question to ask

**Returns:** Streaming response for the resource

**Example:**
```csharp
await foreach (var update in searchService.AskResourceStreamAsync("my-resource-id", "What are the key features?"))
{
    if (update.Item is AnswerContent answerContent && !string.IsNullOrEmpty(answerContent.Text))
    {
        Console.Write(answerContent.Text);
    }
}
```

---

### AskResourceBySlugAsync

Ask a question about a specific resource by slug synchronously.

**Signature:**
```csharp
Task<ApiResponse<SyncAskResponse>> AskResourceBySlugAsync(string slug, AskRequest request)
```

**Parameters:**
- `slug` (string, required): The slug identifier of the resource
- `request` (AskRequest, required): The ask request parameters

**Returns:** Answer specific to the resource identified by slug

**Example:**
```csharp
var askRequest = new AskRequest
{
    Query = "What is the purpose of this document?"
};

var response = await searchService.AskResourceBySlugAsync("my-document-slug", askRequest);
if (response.IsSuccessful)
{
    var result = response.Data;
    Console.WriteLine($"Answer: {result.Answer}");
}
```

---

### PredictProxyAsync

Proxy endpoint that forwards requests to the Predict API with Knowledge Box configuration.

**Signature:**
```csharp
Task<ApiResponse<JsonElement>> PredictProxyAsync(PredictProxiedEndpoints endpoint)
```

**Parameters:**
- `endpoint` (PredictProxiedEndpoints, required): The predict endpoint to call

**Returns:** Response from the predict API

**Example:**
```csharp
var response = await searchService.PredictProxyAsync(PredictProxiedEndpoints.Tokens);
if (response.IsSuccessful)
{
    var result = response.Data;
    Console.WriteLine($"Predict API response: {result}");
}
```

---

### SearchResourceAsync

Search within a specific resource by ID.

**Signature:**
```csharp
Task<ApiResponse<ResourceSearchResults>> SearchResourceAsync(string resourceId, string query)
```

**Parameters:**
- `resourceId` (string, required): The unique identifier of the resource
- `query` (string, required): The search query

**Returns:** Search results within the specified resource

**Example:**
```csharp
var response = await searchService.SearchResourceAsync("my-resource-id", "configuration settings");
if (response.IsSuccessful)
{
    var results = response.Data;
    Console.WriteLine($"Found {results.Total} matches in resource");
    foreach (var paragraph in results.Paragraphs ?? [])
    {
        Console.WriteLine($"Match: {paragraph.Text}");
    }
}
```

## Usage Notes

- All methods are asynchronous and return `ApiResponse<T>` wrappers
- Check `response.IsSuccessful` before accessing `response.Data`
- Streaming methods return `IAsyncEnumerable` for real-time response processing
- For streaming examples, add `using Progress.Nuclia.Model.Streaming;` to access `AnswerContent`
- Ask methods provide RAG capabilities with citations and source information
- Graph search methods enable relationship-based queries
- Resource-specific methods limit scope to individual documents
- Feedback helps improve AI model performance over time
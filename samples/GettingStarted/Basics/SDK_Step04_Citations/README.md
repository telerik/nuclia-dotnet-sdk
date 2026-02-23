# SDK Step 04: Working with Citations

This example shows how to work with citations and retrieve source information for answers, providing transparency and verifiability.

## Prerequisites

Before running this example, you need to set up your Nuclia credentials using .NET user secrets:

```bash
dotnet user-secrets set "ApiKey" "your-api-key-value"
dotnet user-secrets set "ZoneId" "your-zone-id-value"
dotnet user-secrets set "KnowledgeBoxId" "your-knowledge-box-id-value"
```

## Key Concepts

- Enabling citations in `AskRequest` using `Citations = new Citations(true)`
- Processing multiple content types in streaming responses:
  - `AnswerContent` - The answer text
  - `RetrievalContent` - Retrieved resources with metadata
  - `CitationsContent` - Citation mappings
- Correlating citations with their source resources
- Extracting resource metadata (titles, IDs, scores)

## Why This Matters

Citations provide:
- **Transparency**: Users can see where information comes from
- **Verifiability**: Users can verify answers against source materials
- **Trust**: Critical for AI applications in enterprise and regulated environments
- **Traceability**: Track which resources contributed to each answer

## What This Example Does

1. Sets up the client using dependency injection
2. Creates an `AskRequest` with citations enabled
3. Sends a streaming request to the Ask API
4. Processes three types of content as they stream:
   - **Answer text**: Displayed progressively
   - **Retrieved resources**: Stored for later reference
   - **Citations**: Stored for mapping to resources
5. Displays the complete answer
6. Lists all citations with their associated resource IDs and scores
7. Shows all retrieved resources with their titles

## Code Highlights

```csharp
// Enable citations in the request
AskRequest askRequest = new("Write a question about this knowledge box?") 
{ 
    Citations = new Citations(true) 
};

// Variables to collect streaming data
Dictionary<string, FindResource> originalResources = null;
Dictionary<string, int[][]> citations = null;

// Process streaming response
await foreach (var chunk in client.Search.AskStreamAsync(askRequest))
{
    switch (chunk.Item)
    {
        case AnswerContent answer:
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

// Display citations and resources after streaming completes
```

## Understanding Citations Structure

Citations are returned as a dictionary where:
- **Key**: Citation ID (referenced in the answer text)
- **Value**: Array of resource references, where each reference is `[resourceId, score]`

Resources provide additional metadata:
- **Key**: Resource ID
- **Value**: Resource object containing title, fields, and other metadata

## Running This Example

1. Ensure your user secrets are configured (see Prerequisites above)

2. Navigate to this directory:
   ```bash
   cd samples/GettingStarted/Basics/SDK_Step04_Citations
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

You'll see:
1. The answer streaming in real-time
2. A list of citations with their associated resources
3. All retrieved resources with their titles

## Next Steps

Once you understand citations and source attribution, proceed to:
- **SDK_Step05_StructuredOutput**: Learn how to get type-safe, strongly-typed responses from the AI

---

After completing all Basics examples, you'll understand:
- ? Basic connectivity to Nuclia
- ? Dependency injection integration
- ? Streaming responses
- ? Citations and source attribution
- ? Structured outputs

Consider exploring:
- **Blazor samples**: UI integration examples at [SDK_Blazor_Ask](../../Blazor/SDK_Blazor_Ask/)
- **Advanced examples**: More complex scenarios and use cases
- The full SDK documentation for additional features

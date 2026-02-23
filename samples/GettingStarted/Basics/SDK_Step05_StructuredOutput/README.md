# SDK Step 05: Structured Output

This example demonstrates how to get type-safe structured data from AI responses using C# classes and the `[Description]` attribute.

## Prerequisites

Before running this example, you need to set up your Nuclia credentials using .NET user secrets:

```bash
dotnet user-secrets set "ApiKey" "your-api-key-value"
dotnet user-secrets set "ZoneId" "your-zone-id-value"
dotnet user-secrets set "KnowledgeBoxId" "your-knowledge-box-id-value"
```

## Key Concepts

- Using generic `AskAsync<T>()` for type-safe responses
- Defining response schemas with C# classes
- Using `[Description]` attributes to guide AI response generation
- Accessing strongly-typed data directly from responses
- Separating data models into their own files

## Why This Matters

Structured outputs provide:
- **Type Safety**: Compile-time checking of response structure
- **IntelliSense Support**: Auto-completion and type hints in your IDE
- **Predictable Responses**: Guaranteed response format
- **Easier Integration**: Direct mapping to your application's data models
- **Better Validation**: Use C# validation attributes and logic
- **Maintainability**: Clear contracts between AI and application code

## What This Example Does

1. Sets up the client using dependency injection
2. Defines custom classes (`PromptSuggestion` and `Question`) to represent the expected response structure
3. Uses `[Description]` attributes to guide the AI in generating appropriate responses
4. Calls `AskAsync<PromptSuggestion>()` with a generic type parameter
5. Receives a strongly-typed response object
6. Accesses the data properties directly without parsing or casting

## Code Highlights

### Defining the Response Schema

```csharp
public class PromptSuggestion
{
    [Description("An array of suggested questions to ask about the knowledge box")]
    public Question[] Questions { get; set; } = [];
}

public class Question
{
    [Description("A three word title for the question")]
    public string Title { get; set; } = "";
    
    [Description("The full question text that can be used as a search query")]
    public string PromptText { get; set; } = "";
}
```

### Making the Request

```csharp
// Use the generic type parameter to specify expected response structure
var response = await client.Search.AskAsync<PromptSuggestion>(
    new AskRequest("Create three question about the data in this knowledge box.")
);

// Access strongly-typed data
if (response.Data is not null)
{
    foreach (var question in response.Data.Questions)
    {
        Console.WriteLine($"Question: {question.Title} - {question.PromptText}");
    }
}
```

## Understanding the [Description] Attribute

The `[Description]` attribute plays a crucial role in guiding AI response generation:

- **Purpose**: Acts as a hint to Progress Agentic RAG about how to populate each property
- **Function**: The description text is prepended to the property when generating responses
- **Impact**: Changing descriptions directly affects the AI's output

### Example of Description Impact

```csharp
// Original
[Description("A three word title for the question")]
public string Title { get; set; } = "";
// Result: "Knowledge Base Questions", "Data Retrieval Methods"

// Modified
[Description("A FOUR word title for the question")]
public string Title { get; set; } = "";
// Result: "Advanced Knowledge Base Questions", "Efficient Data Retrieval Methods"
```

The AI uses these descriptions as micro-prompts to understand the expected format and content of each field.

## Benefits Over String Responses

Compare this structured approach with parsing string responses:

**Without Structured Output:**
```csharp
var response = await client.Search.AskAsync(request);
// Must parse answer string manually
// Prone to parsing errors
// No compile-time safety
string answer = response.Data.Answer;
```

**With Structured Output:**
```csharp
var response = await client.Search.AskAsync<PromptSuggestion>(request);
// Strongly-typed, validated data
// Compile-time safety
// IntelliSense support
foreach (var question in response.Data.Questions)
{
    Console.WriteLine(question.Title); // Direct property access
}
```

## File Organization

This example demonstrates good practice by separating the data model:

- **Program.cs**: Application logic and SDK usage
- **PromptSuggestion.cs**: Data models (classes and attributes)

This separation improves:
- Code organization
- Reusability of models
- Testability
- Maintainability

## Running This Example

1. Ensure your user secrets are configured (see Prerequisites above)

2. Navigate to this directory:
   ```bash
   cd samples/GettingStarted/Basics/SDK_Step05_StructuredOutput
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

You'll see three AI-generated questions displayed with their titles and full text.

## Experiment with Descriptions

Try modifying the `[Description]` attributes in `PromptSuggestion.cs`:

1. Change "three word" to "five word" in the Title description
2. Add more constraints to the PromptText description
3. Add new properties with their own descriptions
4. Run the program again and observe the different output

This demonstrates how descriptions directly influence AI behavior.

## Next Steps

Once you understand structured outputs, proceed to:
- **SDK_Step06_Ingest**: Learn how to add external content to your knowledge box programmatically

---

After completing all Basics examples, you'll understand:
- ? Basic connectivity to Nuclia
- ? Dependency injection integration
- ? Streaming responses
- ? Citations and source attribution
- ? Structured outputs with type safety
- ? Content ingestion

Consider exploring:
- **Blazor Integration**: See how these concepts apply in [SDK_Blazor_Ask](../../Blazor/SDK_Blazor_Ask/)
- **Advanced Scenarios**: Combine structured outputs with citations and streaming
- **Custom Models**: Create your own complex data structures for specific use cases
- **Validation**: Add C# validation attributes to your structured output classes

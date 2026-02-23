# SDK Blazor Ask

This Blazor Web App demonstrates how to integrate the Progress Nuclia SDK into a Blazor Server application, showcasing three different interaction patterns for working with AI-powered knowledge bases.

## Overview

This sample application includes three pages that demonstrate progressively advanced features:

1. **Home** - Simple request/response pattern
2. **Streaming** - Real-time streaming responses
3. **Structured Output** - Type-safe structured data from AI responses

## Prerequisites

Before running this example, you need to set up your Nuclia credentials using .NET user secrets:

```bash
dotnet user-secrets set "ApiKey" "your-api-key-value"
dotnet user-secrets set "ZoneId" "your-zone-id-value"
dotnet user-secrets set "KnowledgeBoxId" "your-knowledge-box-id-value"
```

## Project Setup

### Program.cs Configuration

The application demonstrates the recommended way to configure the Nuclia SDK in a Blazor application:

```csharp
// Create configuration from app settings/user secrets
NucliaDbConfig config = new(
    ZoneId: builder.Configuration["ZoneId"]!,
    KnowledgeBoxId: builder.Configuration["KnowledgeBoxId"]!,
    ApiKey: builder.Configuration["ApiKey"]!
);

// Register with dependency injection
builder.Services.AddNucliaDb(config);
```

This makes `INucliaDbClient` available for injection into any Blazor component.

## Pages and Features

### 1. Home Page (`/`)

**Purpose**: Demonstrates the basic request/response pattern in a Blazor component.

**Key Concepts**:
- Injecting `INucliaDbClient` into Blazor components
- Using `@inject` directive for dependency injection
- Simple `AskAsync()` calls from Blazor event handlers
- Two-way data binding with `@bind`
- Updating UI after async operations

**What It Does**:
- Provides a text input for user queries
- Sends the query to Nuclia using `AskAsync()`
- Displays the complete answer once received
- Uses Kendo UI CSS utilities for styling

**Code Highlights**:
```csharp
@inject INucliaDbClient NucliaDbClient

async Task HandleSubmit()
{
    answer = "Loading...";
    
    AskRequest askRequest = new AskRequest(query: query);
    var askResponse = await NucliaDbClient.Search.AskAsync(askRequest);
    
    if (askResponse.Success && askResponse?.Data is not null)
    {
        answer = askResponse.Data.Answer;
    }
}
```

---

### 2. Streaming Page (`/streaming`)

**Purpose**: Shows how to implement real-time streaming responses in Blazor for better user experience.

**Key Concepts**:
- Using `AskStreamAsync()` for progressive rendering
- Processing streaming chunks with `await foreach`
- Manually triggering UI updates with `StateHasChanged()`
- Handling asynchronous updates outside normal Blazor event flow
- Progressive text rendering

**What It Does**:
- Provides a text input for user queries
- Streams the answer in real-time as it's generated
- Updates the UI progressively with each chunk
- Demonstrates smooth rendering with small delays

**Code Highlights**:
```csharp
await foreach (var chunk in NucliaDbClient.Search.AskStreamAsync(askRequest))
{
    if (chunk.Item is AnswerContent chunkAnswer)
    {
        answer += chunkAnswer.Text;
        
        // Smooth rendering
        await Task.Delay(1);
        
        // Manual UI refresh for streaming updates
        StateHasChanged();
    }
}
```

**Why `StateHasChanged()` Is Needed**:
Since streaming updates happen outside the normal Blazor event flow, you must manually notify Blazor to refresh the UI to display new content as it arrives.

---

### 3. Structured Output Page (`/structure`)

**Purpose**: Demonstrates how to get type-safe structured data from AI responses using C# classes.

**Key Concepts**:
- Defining response schemas with C# classes
- Using `[Description]` attributes to guide AI responses
- Generic `AskAsync<T>()` for typed responses
- Loading data on component initialization with `OnInitializedAsync()`
- Dynamic UI generation from structured data

**What It Does**:
1. On page load, asks the AI to generate three suggested questions about the knowledge box
2. Receives the suggestions as a strongly-typed `PromptSuggestion` object
3. Displays the suggestions as clickable buttons
4. When a suggestion is clicked, submits that question and displays the answer

**Code Highlights**:
```csharp
public class PromptSuggestion
{
    [Description("An array of suggested questions to ask about the knowledge box")]
    public Question[] Questions { get; set; } = [];
}

public class Question
{
    // The description acts as a hint to Progress Agentic RAG about how to populate this property
    // It's prepended to the property value when generating responses
    // Try changing "three word" to "FOUR word" to see the AI generate longer titles
    [Description("A three word title for the question")]
    public string Title { get; set; } = "";
    
    [Description("The full question text that can be used as a search query")]
    public string PromptText { get; set; } = "";
}

// Use generic type parameter for structured response
var response = await NucliaDbClient.Search.AskAsync<PromptSuggestion>(
    new AskRequest(prompt)
);
```

**Why This Matters**:
Structured outputs enable:
- Type-safe AI responses
- Easier data manipulation and display
- Validation and error handling
- Better integration with existing codebases
- Predictable response formats

**Understanding the [Description] Attribute**:

The `[Description]` attribute is key to guiding AI response generation:
- **Purpose**: Acts as a hint to Progress Agentic RAG about how to populate each property
- **Function**: The description text is prepended to the property when generating responses
- **Impact**: Changing descriptions directly affects the AI's output

Example:
```csharp
// Original: generates "Knowledge Base Questions", "Data Retrieval"
[Description("A three word title for the question")]

// Modified: generates "Advanced Knowledge Base Questions", "Efficient Data Retrieval Methods"
[Description("A FOUR word title for the question")]
```

The AI uses these descriptions as micro-prompts to understand the expected format and content of each field. For more details on structured outputs, see [SDK_Step05_StructuredOutput](../../Basics/SDK_Step05_StructuredOutput/).

---

## Styling

This application uses the **Kendo UI Design System** CSS utility classes for styling:

- `k-display-flex`, `k-flex-col` - Layout utilities
- `k-gap-*` - Spacing utilities
- `k-p-*` - Padding utilities
- `k-border`, `k-rounded-md` - Border utilities
- `k-button`, `k-button-solid`, `k-button-primary` - Button styles
- `k-input`, `k-input-solid` - Input styles
- `k-text-*` - Typography utilities
- `k-font-*` - Font utilities

## Running This Example

1. Ensure your user secrets are configured (see Prerequisites above)

2. Navigate to this directory:
   ```bash
   cd samples/GettingStarted/Blazor/SDK_Blazor_Ask
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Open your browser and navigate to the URL shown (typically `https://localhost:5001`)

5. Explore the three pages:
   - **Home** - Try the basic ask feature
   - **Streaming** - Watch answers stream in real-time
   - **Structured Output** - See AI-generated suggestions

## Key Takeaways

### For Blazor Developers

- ✅ Use `@inject INucliaDbClient` to access the SDK in components
- ✅ Call `StateHasChanged()` when processing streaming responses
- ✅ Use `OnInitializedAsync()` for loading data on page load
- ✅ Leverage two-way binding with `@bind` for input fields
- ✅ Use generic `AskAsync<T>()` for type-safe structured responses

## Architecture Notes

This is a **Blazor Server** application, which means:
- The app runs on the server
- UI updates are sent to the browser via SignalR
- Component state is maintained on the server
- No additional API layer is needed for the Nuclia SDK

The SDK is injected as a **scoped service**, meaning each user connection gets its own instance.

## Next Steps

Consider exploring:
- **Adding citations**: Show source information for answers (similar to SDK_Step04_Citations)
- **Error handling**: Implement robust error handling and user feedback
- **Progress indicators**: Add loading spinners or progress bars
- **Custom styling**: Apply your own theme using Kendo Theme Builder
- **Advanced features**: Explore other Nuclia SDK capabilities like search, filters, and facets

## Related Examples

Before exploring this Blazor example, you may want to review the Basics examples:
- [SDK_Step01_Connecting](../../Basics/SDK_Step01_Connecting/) - Basic connectivity
- [SDK_Step02_Connecting_With_DI](../../Basics/SDK_Step02_Connecting_With_DI/) - Dependency injection
- [SDK_Step03_Streaming_Responses](../../Basics/SDK_Step03_Streaming_Responses/) - Streaming concepts
- [SDK_Step04_Citations](../../Basics/SDK_Step04_Citations/) - Working with citations
- [SDK_Step05_StructuredOutput](../../Basics/SDK_Step05_StructuredOutput/) - Type-safe structured responses
- [SDK_Step06_Ingest](../../Basics/SDK_Step06_Ingest/) - Content ingestion

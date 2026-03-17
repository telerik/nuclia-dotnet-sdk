using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can ask questions to a specific resource with streaming.
/// This test ensures that the SDK can perform resource-scoped Ask operations with IAsyncEnumerable streaming.
/// Expected result: The API call succeeds and returns streaming response chunks.
/// </summary>
public class AskResourceStreamAsyncFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can ask a question to a resource with streaming using AskResourceStreamAsync.
    /// The test passes if streaming chunks are received successfully.
    /// </summary>
    [Fact(DisplayName = "Can ask a question to a resource with streaming using AskResourceStreamAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanAskResourceQuestionWithStreaming()
    {
        // Step 1: Create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-ask-resource-stream-test",
            Summary = "Test resource for streaming Ask with content about machine learning and neural networks",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Ask a question with streaming
        var askRequest = new AskRequest("Summarize this resource");

        var chunks = new System.Collections.Generic.List<Model.Streaming.SyncAskResponseUpdate>();
        await foreach (var chunk in Client.Search.AskResourceStreamAsync(resourceId, askRequest))
        {
            chunks.Add(chunk);
        }

        // Step 3: Verify we received streaming chunks
        Assert.NotEmpty(chunks);
    }

    /// <summary>
    /// Verifies streaming Ask with string query overload.
    /// The test passes if streaming chunks are received successfully.
    /// </summary>
    [Fact(DisplayName = "Can ask a resource with string query and streaming")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanAskResourceWithStringQueryStreaming()
    {
        // Step 1: Create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-ask-string-stream-test",
            Summary = "Test resource for string query streaming",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Ask with string query
        var chunks = new System.Collections.Generic.List<Model.Streaming.SyncAskResponseUpdate>();
        await foreach (var chunk in Client.Search.AskResourceStreamAsync(resourceId, "What is this about?"))
        {
            chunks.Add(chunk);
        }

        // Step 3: Verify chunks received
        Assert.NotEmpty(chunks);
    }
}

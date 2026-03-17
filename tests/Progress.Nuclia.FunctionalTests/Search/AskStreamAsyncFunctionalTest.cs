using System.Linq;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can ask questions with streaming responses.
/// This test ensures that the SDK can handle IAsyncEnumerable streaming of ask responses.
/// Expected result: The API call succeeds and returns streaming chunks.
/// </summary>
public class AskStreamAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can ask a question and receive a streaming response.
    /// The test passes if streaming chunks are received successfully.
    /// </summary>
    [Fact(DisplayName = "Can ask a question with streaming response using AskStreamAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanAskQuestionWithStreaming()
    {
        // Arrange: Create an ask request
        var request = new AskRequest("What is Nuclia?");

        // Act: Send the streaming ask request and collect chunks
        var chunks = new System.Collections.Generic.List<Model.Streaming.SyncAskResponseUpdate>();
        await foreach (var chunk in Client.Search.AskStreamAsync(request))
        {
            chunks.Add(chunk);
        }

        // Assert: Verify we received streaming chunks
        Assert.NotEmpty(chunks);
    }

    /// <summary>
    /// Verifies that the SDK can ask a question using string query with streaming response.
    /// The test passes if streaming chunks are received successfully.
    /// </summary>
    [Fact(DisplayName = "Can ask a question with string query and streaming response")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanAskQuestionWithStringQueryStreaming()
    {
        // Act: Send the streaming ask request with simple string query
        var chunks = new System.Collections.Generic.List<Model.Streaming.SyncAskResponseUpdate>();
        await foreach (var chunk in Client.Search.AskStreamAsync("What is RAG?"))
        {
            chunks.Add(chunk);
        }

        // Assert: Verify we received streaming chunks
        Assert.NotEmpty(chunks);
    }
}

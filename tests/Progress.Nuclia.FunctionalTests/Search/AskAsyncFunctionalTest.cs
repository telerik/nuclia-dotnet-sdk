using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can ask questions to the Knowledge Box synchronously.
/// This test ensures that the SDK can send questions and receive answers using the Ask endpoint.
/// Expected result: The API call succeeds and returns a valid answer.
/// </summary>
public class AskAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can ask a question synchronously and receive an answer.
    /// The test passes if the API call succeeds and returns a response with an answer.
    /// </summary>
    [Fact(DisplayName = "Can ask a question synchronously using AskAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanAskQuestionSynchronously()
    {
        // Arrange: Create an ask request
        var request = new AskRequest("What is Nuclia?");

        // Act: Send the ask request
        var response = await Client.Search.AskAsync(request);

        // Assert: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Ask request failed: {response.Error}");
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Answer);
    }

    /// <summary>
    /// Verifies that the SDK can ask a question and receive a typed response.
    /// The test passes if the API call succeeds and returns a structured response.
    /// </summary>
    [Fact(DisplayName = "Can ask a question with typed response using AskAsync<T>")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanAskQuestionWithTypedResponse()
    {
        // Arrange: Define a simple response type
        var request = new AskRequest("List three facts about RAG");

        // Act: Send the ask request with typed response
        var response = await Client.Search.AskAsync<SimpleAnswer>(request);

        // Assert: Verify the typed response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Typed ask request failed: {response.Error}");
        Assert.NotNull(response.Data);
    }

    private class SimpleAnswer
    {
        public string Facts { get; set; } = string.Empty;
    }
}

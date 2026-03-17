using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can search the Knowledge Box.
/// This test ensures that the SDK can perform comprehensive searches with documents, paragraphs, and sentences.
/// Expected result: The API call succeeds and returns search results.
/// </summary>
public class SearchAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can search the Knowledge Box using SearchAsync.
    /// The test passes if the API call succeeds and returns results.
    /// </summary>
    [Fact(DisplayName = "Can search Knowledge Box using SearchAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanSearchKnowledgeBox()
    {
        // Arrange: Create a search request
        var request = new SearchRequest
        {
            Query = "test"
        };

        // Act: Execute the search request
        var response = await Client.Search.SearchAsync(request);

        // Assert: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Search request failed: {response.Error}");
        Assert.NotNull(response.Data);
    }
}

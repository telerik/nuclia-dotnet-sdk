using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can retrieve suggestions for a query.
/// This test ensures that the SDK can get paragraph and entity suggestions.
/// Expected result: The API call succeeds and returns suggestion results.
/// </summary>
public class SuggestAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can get suggestions using SuggestAsync.
    /// The test passes if the API call succeeds and returns suggestions.
    /// </summary>
    [Fact(DisplayName = "Can get suggestions using SuggestAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanGetSuggestions()
    {
        // Arrange: Define a query for suggestions
        var query = "test";

        // Act: Execute the suggest request
        var response = await Client.Search.SuggestAsync(query);

        // Assert: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Suggest request failed: {response.Error}");
        Assert.NotNull(response.Data);
    }
}

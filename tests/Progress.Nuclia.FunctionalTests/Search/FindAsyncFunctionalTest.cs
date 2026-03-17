using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can find resources in the Knowledge Box.
/// This test ensures that the SDK can perform find operations with detailed paragraph information.
/// Expected result: The API call succeeds and returns find results.
/// </summary>
public class FindAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can find resources using FindAsync.
    /// The test passes if the API call succeeds and returns results.
    /// </summary>
    [Fact(DisplayName = "Can find resources using FindAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanFindResources()
    {
        // Arrange: Create a find request
        var request = new FindRequest
        {
            Query = "test"
        };

        // Act: Execute the find request
        var response = await Client.Search.FindAsync(request);

        // Assert: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Find request failed: {response.Error}");
        Assert.NotNull(response.Data);
    }
}

using System.Linq;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can summarize resources.
/// This test ensures that the SDK can generate summaries for specified resources.
/// Expected result: The API call succeeds and returns a summary.
/// </summary>
public class SummarizeAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can summarize resources using SummarizeAsync.
    /// The test passes if the API call succeeds and returns a summary response.
    /// </summary>
    [Fact(DisplayName = "Can summarize resources using SummarizeAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanSummarizeResources()
    {
        // Arrange: Fetch available resources from catalog
        var catalogRequest = new CatalogRequest
        {
            PageSize = 1 // We only need one resource to test summarization
        };
        var catalogResponse = await Client.Search.CatalogAsync(catalogRequest);

        // Skip test if no resources are available
        if (catalogResponse.Data?.Resources == null || catalogResponse.Data.Resources.Count == 0)
        {
            // Skip test - no resources available to summarize
            return;
        }

        // Get the first resource ID/slug
        var resourceId = catalogResponse.Data.Resources.Keys.First();
        var request = new SummarizeRequest(new System.Collections.Generic.List<string> { resourceId });

        // Act: Execute the summarize request
        var response = await Client.Search.SummarizeAsync(request);

        // Assert: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Summarize request failed: {response.Error}");
        Assert.NotNull(response.Data);
    }
}

using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can list resources using the Catalog endpoint.
/// This test ensures that the SDK can retrieve cataloged resources from the Knowledge Box.
/// Expected result: The API call succeeds and returns catalog results.
/// </summary>
public class CatalogAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can catalog resources using CatalogAsync.
    /// The test passes if the API call succeeds and returns results.
    /// </summary>
    [Fact(DisplayName = "Can catalog resources using CatalogAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanCatalogResources()
    {
        // Arrange: Create a catalog request
        var request = new CatalogRequest();

        // Act: Execute the catalog request
        var response = await Client.Search.CatalogAsync(request);

        // Assert: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Catalog request failed: {response.Error}");
        Assert.NotNull(response.Data);
    }
}

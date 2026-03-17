using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can search graph relations (edges).
/// This test ensures that the SDK can retrieve relations from the Knowledge Box graph.
/// Expected result: The API call succeeds and returns relation search results.
/// </summary>
public class GraphRelationsSearchAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can search graph relations using GraphRelationsSearchAsync.
    /// The test passes if the API call succeeds and returns relation results.
    /// </summary>
    [Fact(DisplayName = "Can search graph relations using GraphRelationsSearchAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanSearchGraphRelations()
    {
        // Arrange: Create a graph relations search request with a proper query
        var query = new GraphRelationsQuery
        {
            NucliadbModelsGraphRequestsRelation = new NucliadbModelsGraphRequestsRelation
            {
                Prop = NucliadbModelsGraphRequestsRelation.PropEnum.Relation
            }
        };
        var request = new GraphRelationsSearchRequest(query);

        // Act: Execute the graph relations search request
        var response = await Client.Search.GraphRelationsSearchAsync(request);

        // Assert: Verify the response
        Assert.NotNull(response);
        var errorMessage = response.Error ?? response.ValidationError?.ToString();
        Assert.True(response.Success, $"Graph relations search request failed: {errorMessage}");
        Assert.NotNull(response.Data);
    }
}

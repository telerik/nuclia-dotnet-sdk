using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can search graph nodes (vertices).
/// This test ensures that the SDK can retrieve nodes from the Knowledge Box graph.
/// Expected result: The API call succeeds and returns node search results.
/// </summary>
public class GraphNodesSearchAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can search graph nodes using GraphNodesSearchAsync.
    /// The test passes if the API call succeeds and returns node results.
    /// </summary>
    [Fact(DisplayName = "Can search graph nodes using GraphNodesSearchAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanSearchGraphNodes()
    {
        // Arrange: Create a graph nodes search request with AnyNode query
        var query = new GraphNodesQuery
        {
            AnyNode = new AnyNode
            {
                Prop = AnyNode.PropEnum.Node
            }
        };
        var request = new GraphNodesSearchRequest(query);

        // Act: Execute the graph nodes search request
        var response = await Client.Search.GraphNodesSearchAsync(request);

        // Assert: Verify the response
        Assert.NotNull(response);
        var errorMessage = response.Error ?? response.ValidationError?.ToString();
        Assert.True(response.Success, $"Graph nodes search request failed: {errorMessage}");
        Assert.NotNull(response.Data);
    }
}

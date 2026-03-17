using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can search the Knowledge Box graph.
/// This test ensures that the SDK can perform graph searches (triplets vertex-edge-vertex).
/// Expected result: The API call succeeds and returns graph search results.
/// </summary>
public class GraphSearchAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can search the graph using GraphSearchAsync.
    /// The test passes if the API call succeeds and returns graph results.
    /// </summary>
    [Fact(DisplayName = "Can search graph using GraphSearchAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanSearchGraph()
    {
        // Arrange: Create a graph search request with AnyNode query
        var query = new GraphPathQuery
        {
            AnyNode = new AnyNode
            {
                Prop = AnyNode.PropEnum.Node
            }
        };
        var request = new GraphSearchRequest(query);

        // Act: Execute the graph search request
        var response = await Client.Search.GraphSearchAsync(request);

        // Assert: Verify the response
        Assert.NotNull(response);
        var errorMessage = response.Error ?? response.ValidationError?.ToString();
        Assert.True(response.Success, $"Graph search request failed: {errorMessage}");
        Assert.NotNull(response.Data);
    }
}

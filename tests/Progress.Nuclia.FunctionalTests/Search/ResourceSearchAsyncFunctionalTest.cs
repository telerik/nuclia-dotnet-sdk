using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can search within a specific resource.
/// This test ensures that the SDK can perform resource-scoped search operations.
/// Expected result: The API call succeeds and returns search results within the resource.
/// </summary>
public class ResourceSearchAsyncFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can search within a resource using ResourceSearchAsync.
    /// The test passes if the API call succeeds and returns results.
    /// </summary>
    [Fact(DisplayName = "Can search within a resource using ResourceSearchAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanSearchWithinResource()
    {
        // Step 1: Create a test resource with searchable content
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-resource-search-test",
            Summary = "Test resource for resource search functionality with unique searchable keywords",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Search within the resource
        var searchQuery = "searchable";
        var response = await Client.Search.ResourceSearchAsync(resourceId, searchQuery);

        // Step 3: Verify the search response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Resource search failed: {response.Error}");
        Assert.NotNull(response.Data);
    }
}

using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to verify that the Nuclia SDK can search within a single resource by its ID.
/// This test ensures that the SDK can perform resource-scoped search operations.
/// Expected result: The API call succeeds and returns search results.
/// </summary>
public class SearchWithinResourceByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can search within a resource by its ID.
    /// The test passes if the API call succeeds and returns search results.
    /// </summary>
    [Fact(DisplayName = "Can search within a resource by ID using SearchWithinResourceByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanSearchWithinResourceById()
    {
        // Step 1: Create a test resource with searchable content
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-search-within-test",
            Summary = "Test resource for search within functionality with unique searchable content",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Search within the resource
        var searchQuery = "searchable";
        var searchResponse = await Client.Resources.SearchWithinResourceByIdAsync(
            resourceId,
            searchQuery
        );

        // Step 3: Verify search response
        Assert.NotNull(searchResponse);
        Assert.True(searchResponse.Success, $"Search within resource failed: {searchResponse.Error}");
        Assert.NotNull(searchResponse.Data);
    }

    /// <summary>
    /// Verifies that the SDK can search within a resource with additional parameters.
    /// The test passes if the API call succeeds with filters and options.
    /// </summary>
    [Fact(DisplayName = "Can search within resource with parameters by ID")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanSearchWithinResourceWithParametersById()
    {
        // Step 1: Create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-search-params-test",
            Summary = "Test resource for parameterized search functionality",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Search with parameters
        var searchResponse = await Client.Resources.SearchWithinResourceByIdAsync(
            resourceId,
            query: "test",
            topK: 10,
            highlight: true
        );

        // Step 3: Verify search response
        Assert.NotNull(searchResponse);
        Assert.True(searchResponse.Success, $"Parameterized search failed: {searchResponse.Error}");
        Assert.NotNull(searchResponse.Data);
    }
}

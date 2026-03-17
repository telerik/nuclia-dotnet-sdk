using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to verify that the Nuclia SDK can reindex a resource by its ID.
/// This test ensures that the SDK can trigger a background reindex operation for a resource.
/// Expected result: The API call succeeds and returns true indicating the reindex was accepted.
/// </summary>
public class ReindexResourceByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can reindex a resource by its ID.
    /// The test passes if the API call succeeds and returns true.
    /// </summary>
    [Fact(DisplayName = "Can reindex a resource by ID using ReindexResourceByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanReindexResourceById()
    {
        // Step 1: Create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-reindex-test",
            Summary = "Test resource for reindex operation",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Reindex the resource
        var reindexResponse = await Client.Resources.ReindexResourceByIdAsync(resourceId);

        // Step 3: Verify reindex was accepted
        Assert.NotNull(reindexResponse);
        Assert.True(reindexResponse.Success, $"Reindex operation failed: {reindexResponse.Error}");
        Assert.True(reindexResponse.Data, "Reindex should return true when accepted");
    }

    /// <summary>
    /// Verifies that the SDK can reindex a resource including vectors by its ID.
    /// The test passes if the API call succeeds with reindexVectors parameter.
    /// </summary>
    [Fact(DisplayName = "Can reindex resource with vectors by ID")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanReindexResourceWithVectorsById()
    {
        // Step 1: Create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-reindex-vectors-test",
            Summary = "Test resource for reindex with vectors",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Reindex the resource including vectors
        var reindexResponse = await Client.Resources.ReindexResourceByIdAsync(resourceId, reindexVectors: true);

        // Step 3: Verify reindex was accepted
        Assert.NotNull(reindexResponse);
        Assert.True(reindexResponse.Success, $"Reindex operation failed: {reindexResponse.Error}");
        Assert.True(reindexResponse.Data, "Reindex should return true when accepted");
    }
}

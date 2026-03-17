using System;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to verify that the Nuclia SDK can reindex a resource by its slug.
/// This test ensures that the SDK can trigger a background reindex operation for a resource using its slug.
/// Expected result: The API call succeeds and returns true indicating the reindex was accepted.
/// </summary>
public class ReindexResourceBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can reindex a resource by its slug.
    /// The test passes if the API call succeeds and returns true.
    /// </summary>
    [Fact(DisplayName = "Can reindex a resource by slug using ReindexResourceBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanReindexResourceBySlug()
    {
        // Step 1: Create a test resource with a slug
        var testSlug = $"test-slug-reindex-{Guid.NewGuid()}";
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-reindex-slug-test",
            Summary = "Test resource for reindex by slug operation",
            Slug = testSlug
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Reindex the resource by slug
        var reindexResponse = await Client.Resources.ReindexResourceBySlugAsync(testSlug);

        // Step 3: Verify reindex was accepted
        Assert.NotNull(reindexResponse);
        Assert.True(reindexResponse.Success, $"Reindex operation failed: {reindexResponse.Error}");
        Assert.True(reindexResponse.Data, "Reindex should return true when accepted");
    }
}

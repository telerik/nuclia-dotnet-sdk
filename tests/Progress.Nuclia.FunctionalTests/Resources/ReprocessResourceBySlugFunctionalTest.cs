using System;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to verify that the Nuclia SDK can reprocess a resource by its slug.
/// This test ensures that the SDK can trigger resource reprocessing using slug and receive update information.
/// Expected result: The API call succeeds and returns ResourceUpdated information.
/// </summary>
public class ReprocessResourceBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can reprocess a resource by its slug.
    /// The test passes if the API call succeeds and returns ResourceUpdated data.
    /// </summary>
    [Fact(DisplayName = "Can reprocess a resource by slug using ReprocessResourceBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanReprocessResourceBySlug()
    {
        // Step 1: Create a test resource with a slug
        var testSlug = $"test-slug-reprocess-{Guid.NewGuid()}";
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-reprocess-slug-test",
            Summary = "Test resource for reprocess by slug operation",
            Slug = testSlug
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Reprocess the resource by slug
        var reprocessResponse = await Client.Resources.ReprocessResourceBySlugAsync(testSlug);

        // Step 3: Verify reprocess was accepted
        Assert.NotNull(reprocessResponse);
        Assert.True(reprocessResponse.Success, $"Reprocess operation failed: {reprocessResponse.Error}");
        Assert.NotNull(reprocessResponse.Data);
    }
}

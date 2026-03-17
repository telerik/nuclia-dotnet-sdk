using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to verify that the Nuclia SDK can reprocess a resource by its ID.
/// This test ensures that the SDK can trigger resource reprocessing and receive update information.
/// Expected result: The API call succeeds and returns ResourceUpdated information.
/// </summary>
public class ReprocessResourceByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can reprocess a resource by its ID.
    /// The test passes if the API call succeeds and returns ResourceUpdated data.
    /// </summary>
    [Fact(DisplayName = "Can reprocess a resource by ID using ReprocessResourceByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanReprocessResourceById()
    {
        // Step 1: Create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-reprocess-test",
            Summary = "Test resource for reprocess operation",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Reprocess the resource
        var reprocessResponse = await Client.Resources.ReprocessResourceByIdAsync(resourceId);

        // Step 3: Verify reprocess was accepted
        Assert.NotNull(reprocessResponse);
        Assert.True(reprocessResponse.Success, $"Reprocess operation failed: {reprocessResponse.Error}");
        Assert.NotNull(reprocessResponse.Data);
    }

    /// <summary>
    /// Verifies that the SDK can reprocess a resource with title reset by its ID.
    /// The test passes if the API call succeeds with resetTitle parameter.
    /// </summary>
    [Fact(DisplayName = "Can reprocess resource with title reset by ID")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanReprocessResourceWithTitleResetById()
    {
        // Step 1: Create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-reprocess-reset-test",
            Summary = "Test resource for reprocess with title reset",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Reprocess the resource with title reset
        var reprocessResponse = await Client.Resources.ReprocessResourceByIdAsync(resourceId, resetTitle: true);

        // Step 3: Verify reprocess was accepted
        Assert.NotNull(reprocessResponse);
        Assert.True(reprocessResponse.Success, $"Reprocess operation failed: {reprocessResponse.Error}");
        Assert.NotNull(reprocessResponse.Data);
    }
}

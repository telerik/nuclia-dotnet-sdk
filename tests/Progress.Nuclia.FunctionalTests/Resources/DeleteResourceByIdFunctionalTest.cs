using System;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to verify that the Nuclia SDK can delete a resource by ID in the configured test Knowledge Box.
/// This test ensures that the SDK is able to connect to the real API using provided credentials and successfully delete a resource.
/// Expected result: The resource is deleted and the API returns a success status.
/// </summary>
public class DeleteResourceByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can delete a resource by its ID.
    /// The test passes if the resource is successfully created and then deleted, returning a success status.
    /// </summary>
    [Fact(DisplayName = "Can delete a resource by ID using DeleteResourceByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanDeleteResourceById()
    {
        // Step 1: Create a test resource
        var payload = new CreateResourcePayload
        {
            Title = $"test-delete-{Guid.NewGuid()}",
            Summary = "Test resource for deletion functional test"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(payload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        Assert.False(string.IsNullOrEmpty(resourceId), "Returned UUID is null or empty");
        
        // Track for cleanup in case deletion fails
        CreatedResourceIds.Add(resourceId);

        // Step 2: Delete the resource
        var deleteResponse = await Client.Resources.DeleteResourceByIdAsync(resourceId);
        
        // Step 3: Verify deletion succeeded
        Assert.NotNull(deleteResponse);
        Assert.True(deleteResponse.Success, $"Resource deletion failed: {deleteResponse.Error}");
        Assert.True(deleteResponse.Data, "Deletion should return true on success");
        
        // Remove from cleanup list since it was successfully deleted
        CreatedResourceIds.Remove(resourceId);

        // Step 4: Verify the resource no longer exists
        var getResponse = await Client.Resources.GetResourceByIdAsync(resourceId);
        Assert.False(getResponse.Success, "Getting a deleted resource should fail");
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Progress.Nuclia;
using Progress.Nuclia.Model;

/// <summary>
/// Functional test to verify that the Nuclia SDK can delete a resource by ID in the configured test Knowledge Box.
/// This test ensures that the SDK is able to connect to the real API using provided credentials and successfully delete a resource.
/// Expected result: The resource is deleted and the API returns a success status.
/// </summary>
public class DeleteResourceByIdFunctionalTest : IAsyncLifetime
{
    private readonly NucliaDbClient _client;
    private readonly List<string> _createdResourceIds = new();

    public DeleteResourceByIdFunctionalTest()
    {
        var zoneId = Environment.GetEnvironmentVariable("NUCLIA_ZONE_ID");
        var kbId = Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        var apiKey = Environment.GetEnvironmentVariable("NUCLIA_API_KEY");

        if (string.IsNullOrEmpty(zoneId) || string.IsNullOrEmpty(kbId) || string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Missing Nuclia API credentials. Set NUCLIA_ZONE_ID, NUCLIA_KB_ID, NUCLIA_API_KEY.");
        }

        var config = new NucliaDbConfig(zoneId, kbId, apiKey);
        _client = new NucliaDbClient(config);
    }

    /// <summary>
    /// Initialize async - no setup required.
    /// </summary>
    public Task InitializeAsync() => Task.CompletedTask;

    /// <summary>
    /// Clean up all created resources after tests complete.
    /// </summary>
    public async Task DisposeAsync()
    {
        foreach (var resourceId in _createdResourceIds)
        {
            try
            {
                await _client.Resources.DeleteResourceByIdAsync(resourceId);
            }
            catch
            {
                // Ignore cleanup errors to prevent test failures
            }
        }
    }

    /// <summary>
    /// Verifies that the SDK can delete a resource by its ID.
    /// The test passes if the resource is successfully created and then deleted, returning a success status.
    /// </summary>
    [Fact(DisplayName = "Can delete a resource by ID using DeleteResourceByIdAsync")]
    [Trait("Category", "Functional")]
    public async Task CanDeleteResourceById()
    {
        // Step 1: Create a test resource
        var payload = new CreateResourcePayload
        {
            Title = $"test-delete-{Guid.NewGuid()}",
            Summary = "Test resource for deletion functional test"
        };

        var createResponse = await _client.Resources.CreateResourceAsync(payload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        Assert.False(string.IsNullOrEmpty(resourceId), "Returned UUID is null or empty");
        
        // Track for cleanup in case deletion fails
        _createdResourceIds.Add(resourceId);

        // Step 2: Delete the resource
        var deleteResponse = await _client.Resources.DeleteResourceByIdAsync(resourceId);
        
        // Step 3: Verify deletion succeeded
        Assert.NotNull(deleteResponse);
        Assert.True(deleteResponse.Success, $"Resource deletion failed: {deleteResponse.Error}");
        Assert.True(deleteResponse.Data, "Deletion should return true on success");
        
        // Remove from cleanup list since it was successfully deleted
        _createdResourceIds.Remove(resourceId);

        // Step 4: Verify the resource no longer exists
        var getResponse = await _client.Resources.GetResourceByIdAsync(resourceId);
        Assert.False(getResponse.Success, "Getting a deleted resource should fail");
    }
}

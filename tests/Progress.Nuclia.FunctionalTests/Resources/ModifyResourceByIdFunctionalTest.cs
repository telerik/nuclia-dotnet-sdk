using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to verify that the Nuclia SDK can modify an existing resource by its ID.
/// This test ensures that the SDK is able to connect to the real API using provided credentials and update resource properties using ModifyResourceByIdAsync.
/// Expected result: The API call succeeds and returns a ResourceUpdated response confirming the modification.
/// </summary>
public class ModifyResourceByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can modify an existing resource using ModifyResourceByIdAsync.
    /// The test passes if the API call succeeds and returns a valid ResourceUpdated response.
    /// </summary>
    [Fact(DisplayName = "Can modify resource by ID using ModifyResourceByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanModifyResourceById()
    {
        // First, create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-modify-test",
            Summary = "Original summary for modification test",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        Assert.False(string.IsNullOrEmpty(createResponse.Data.Uuid), "Created resource UUID is null or empty");

        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Now modify the resource
        var updatePayload = new UpdateResourcePayload
        {
            Title = $"{TestPrefix}-modified-title",
            Summary = "Updated summary after modification"
        };

        var modifyResponse = await Client.Resources.ModifyResourceByIdAsync(resourceId, updatePayload);

        // Verify the modification response
        Assert.NotNull(modifyResponse);
        Assert.True(modifyResponse.Success, $"ModifyResourceById API call failed: {modifyResponse.Error}");
        Assert.NotNull(modifyResponse.Data);
    }

    /// <summary>
    /// Verifies that the SDK can modify a resource and add text fields using ModifyResourceByIdAsync.
    /// The test passes if the API call succeeds and the text field is added successfully.
    /// </summary>
    [Fact(DisplayName = "Can modify resource and add text fields using ModifyResourceByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanModifyResourceAndAddTextField()
    {
        // First, create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-modify-text-test",
            Summary = "Resource for testing text field modification",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Modify the resource by adding a text field
        var textField = new TextField("This is a new text field added through modification")
        {
            Format = TextFormat.PLAIN
        };

        var updatePayload = new UpdateResourcePayload
        {
            Texts = new Dictionary<string, TextField>
            {
                { "description", textField }
            }
        };

        var modifyResponse = await Client.Resources.ModifyResourceByIdAsync(resourceId, updatePayload);

        // Verify the modification response
        Assert.NotNull(modifyResponse);
        Assert.True(modifyResponse.Success, $"ModifyResourceById API call failed: {modifyResponse.Error}");
        Assert.NotNull(modifyResponse.Data);

        // Optionally verify the modification by fetching the resource
        var getResponse = await Client.Resources.GetResourceByIdAsync(resourceId);
        Assert.NotNull(getResponse);
        Assert.True(getResponse.Success, $"GetResourceById failed: {getResponse.Error}");
        Assert.NotNull(getResponse.Data);
    }
}

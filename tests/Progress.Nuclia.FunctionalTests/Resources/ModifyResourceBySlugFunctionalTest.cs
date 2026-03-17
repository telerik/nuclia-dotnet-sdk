using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to verify that the Nuclia SDK can modify a resource by its slug.
/// This test ensures that the SDK can update resource properties using the slug identifier.
/// Expected result: The API call succeeds and returns a ResourceUpdated response confirming the modification.
/// </summary>
public class ModifyResourceBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can modify a resource using its slug.
    /// The test passes if the API call succeeds and returns a valid ResourceUpdated response.
    /// </summary>
    [Fact(DisplayName = "Can modify resource by slug using ModifyResourceBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanModifyResourceBySlug()
    {
        // Step 1: Create a test resource with a slug
        var testSlug = $"test-slug-modify-{Guid.NewGuid()}";
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-modify-slug-test",
            Summary = "Original summary for modification by slug test",
            Slug = testSlug,
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Modify the resource by slug
        var updatePayload = new UpdateResourcePayload
        {
            Title = $"{TestPrefix}-modified-title-slug",
            Summary = "Updated summary after modification by slug"
        };

        var modifyResponse = await Client.Resources.ModifyResourceBySlugAsync(testSlug, updatePayload);

        // Step 3: Verify the modification response
        Assert.NotNull(modifyResponse);
        Assert.True(modifyResponse.Success, $"ModifyResourceBySlug API call failed: {modifyResponse.Error}");
        Assert.NotNull(modifyResponse.Data);
    }

    /// <summary>
    /// Verifies that the SDK can modify a resource and add text fields using slug.
    /// The test passes if the API call succeeds and the text field is added successfully.
    /// </summary>
    [Fact(DisplayName = "Can modify resource and add text fields using slug")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanModifyResourceAndAddTextFieldBySlug()
    {
        // Step 1: Create a test resource with a slug
        var testSlug = $"test-slug-modify-text-{Guid.NewGuid()}";
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-modify-text-slug-test",
            Summary = "Resource for testing text field modification by slug",
            Slug = testSlug,
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Modify the resource by slug, adding a text field
        var textField = new TextField("This is a new text field added through modification by slug")
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

        var modifyResponse = await Client.Resources.ModifyResourceBySlugAsync(testSlug, updatePayload);

        // Step 3: Verify the modification response
        Assert.NotNull(modifyResponse);
        Assert.True(modifyResponse.Success, $"ModifyResourceBySlug API call failed: {modifyResponse.Error}");
        Assert.NotNull(modifyResponse.Data);
    }
}

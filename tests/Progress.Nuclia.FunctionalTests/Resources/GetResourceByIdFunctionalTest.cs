using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to verify that the Nuclia SDK can retrieve a resource by its ID.
/// This test ensures that the SDK is able to connect to the real API using provided credentials and fetch resource details using GetResourceByIdAsync.
/// Expected result: The API call succeeds and returns valid resource data matching the created resource.
/// </summary>
public class GetResourceByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can retrieve a resource by its ID using GetResourceByIdAsync.
    /// The test passes if the API call succeeds and returns the expected resource data.
    /// </summary>
    [Fact(DisplayName = "Can get resource by ID using GetResourceByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanGetResourceById()
    {
        // First, create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-get-test",
            Summary = "Test resource for GetResourceById functional test",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        Assert.False(string.IsNullOrEmpty(createResponse.Data.Uuid), "Created resource UUID is null or empty");

        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Now retrieve the resource by its ID
        var getResponse = await Client.Resources.GetResourceByIdAsync(resourceId);

        // Verify the response
        Assert.NotNull(getResponse);
        Assert.True(getResponse.Success, $"GetResourceById API call failed: {getResponse.Error}");
        Assert.NotNull(getResponse.Data);
        Assert.Equal(resourceId, getResponse.Data.Id);
        Assert.NotNull(getResponse.Data.Title);
    }

    /// <summary>
    /// Verifies that the SDK can retrieve a resource with specific properties using show parameter.
    /// The test passes if the API call succeeds and returns resource data with requested properties.
    /// </summary>
    [Fact(DisplayName = "Can get resource by ID with specific properties")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanGetResourceByIdWithProperties()
    {
        // First, create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-get-props-test",
            Summary = "Test resource for GetResourceById with properties",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Retrieve the resource with specific properties
        var getResponse = await Client.Resources.GetResourceByIdAsync(
            resourceId,
            show: new[] { ResourceProperties.Basic, ResourceProperties.Values }
        );

        // Verify the response
        Assert.NotNull(getResponse);
        Assert.True(getResponse.Success, $"GetResourceById API call failed: {getResponse.Error}");
        Assert.NotNull(getResponse.Data);
        Assert.Equal(resourceId, getResponse.Data.Id);
    }
}

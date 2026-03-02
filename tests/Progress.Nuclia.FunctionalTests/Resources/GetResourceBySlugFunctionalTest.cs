using System;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to verify that the Nuclia SDK can retrieve a resource by its slug from the configured test Knowledge Box.
/// This test ensures that the SDK is able to connect to the real API using provided credentials and retrieve a resource using GetResourceBySlugAsync.
/// Expected result: The API call succeeds, returns the resource with matching slug, and contains the expected data.
/// </summary>
public class GetResourceBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    private readonly string _testSlug;

    public GetResourceBySlugFunctionalTest()
    {
        _testSlug = "test-slug-" + Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Verifies that the SDK can retrieve a resource by its slug.
    /// The test passes if the API returns the resource matching the specified slug with correct data.
    /// </summary>
    [Fact(DisplayName = "Can retrieve a resource by slug")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanGetResourceBySlug()
    {
        // Arrange: Create a resource with a specific slug
        var createPayload = new CreateResourcePayload
        {
            Title = "Test Resource for Slug Retrieval",
            Summary = "This resource tests GetResourceBySlugAsync functionality",
            Slug = _testSlug
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.NotNull(createResponse);
        Assert.True(createResponse.Success, $"Failed to create test resource: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);

        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Act: Retrieve the resource by slug
        var getResponse = await Client.Resources.GetResourceBySlugAsync(_testSlug);

        // Assert: Verify the response is successful and contains expected data
        Assert.NotNull(getResponse);
        Assert.True(getResponse.Success, $"API call failed: {getResponse.Error}");
        Assert.NotNull(getResponse.Data);
        
        // Verify the retrieved resource has the correct slug and ID
        Assert.Equal(_testSlug, getResponse.Data.Slug);
        Assert.Equal(resourceId, getResponse.Data.Id);
        Assert.Equal(createPayload.Title, getResponse.Data.Title);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia;
using Progress.Nuclia.Model;
using Xunit;

/// <summary>
/// Functional test to verify that the Nuclia SDK can retrieve a resource by its slug from the configured test Knowledge Box.
/// This test ensures that the SDK is able to connect to the real API using provided credentials and retrieve a resource using GetResourceBySlugAsync.
/// Expected result: The API call succeeds, returns the resource with matching slug, and contains the expected data.
/// </summary>
public class GetResourceBySlugFunctionalTest : IAsyncLifetime
{
    private readonly NucliaDbClient _client;
    private readonly string _testSlug;
    private readonly List<string> _createdResourceIds = new();

    /// <summary>
    /// Initializes the test client using environment variables for credentials.
    /// Throws if required credentials are missing.
    /// </summary>
    public GetResourceBySlugFunctionalTest()
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
        _testSlug = "test-slug-" + Guid.NewGuid().ToString();
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
    /// Verifies that the SDK can retrieve a resource by its slug.
    /// The test passes if the API returns the resource matching the specified slug with correct data.
    /// </summary>
    [Fact(DisplayName = "Can retrieve a resource by slug")]
    [Trait("Category", "Functional")]
    public async Task CanGetResourceBySlug()
    {
        // Arrange: Create a resource with a specific slug
        var createPayload = new CreateResourcePayload
        {
            Title = "Test Resource for Slug Retrieval",
            Summary = "This resource tests GetResourceBySlugAsync functionality",
            Slug = _testSlug
        };

        var createResponse = await _client.Resources.CreateResourceAsync(createPayload);
        Assert.NotNull(createResponse);
        Assert.True(createResponse.Success, $"Failed to create test resource: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);

        var resourceId = createResponse.Data.Uuid;
        _createdResourceIds.Add(resourceId);

        // Act: Retrieve the resource by slug
        var getResponse = await _client.Resources.GetResourceBySlugAsync(_testSlug);

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

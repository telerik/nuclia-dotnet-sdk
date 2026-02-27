using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia;
using Progress.Nuclia.Model;
using Xunit;

/// <summary>
/// Functional test to verify that the Nuclia SDK can retrieve a resource by its ID.
/// This test ensures that the SDK is able to connect to the real API using provided credentials and fetch resource details using GetResourceByIdAsync.
/// Expected result: The API call succeeds and returns valid resource data matching the created resource.
/// </summary>
public class GetResourceByIdFunctionalTest : IAsyncLifetime
{
    private readonly NucliaDbClient _client;
    private readonly string _testPrefix;
    private readonly List<string> _createdResourceIds = new();

    /// <summary>
    /// Initializes the test client using environment variables for credentials.
    /// Throws if required credentials are missing.
    /// </summary>
    public GetResourceByIdFunctionalTest()
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
        _testPrefix = "test-" + Guid.NewGuid().ToString();
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
                // Ignore cleanup errors
            }
        }
    }

    /// <summary>
    /// Verifies that the SDK can retrieve a resource by its ID using GetResourceByIdAsync.
    /// The test passes if the API call succeeds and returns the expected resource data.
    /// </summary>
    [Fact(DisplayName = "Can get resource by ID using GetResourceByIdAsync")]
    [Trait("Category", "Functional")]
    public async Task CanGetResourceById()
    {
        // First, create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{_testPrefix}-get-test",
            Summary = "Test resource for GetResourceById functional test",
            Icon = "application/stf-link"
        };

        var createResponse = await _client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        Assert.False(string.IsNullOrEmpty(createResponse.Data.Uuid), "Created resource UUID is null or empty");

        var resourceId = createResponse.Data.Uuid;
        _createdResourceIds.Add(resourceId);

        // Now retrieve the resource by its ID
        var getResponse = await _client.Resources.GetResourceByIdAsync(resourceId);

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
    public async Task CanGetResourceByIdWithProperties()
    {
        // First, create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{_testPrefix}-get-props-test",
            Summary = "Test resource for GetResourceById with properties",
            Icon = "application/stf-link"
        };

        var createResponse = await _client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        var resourceId = createResponse.Data.Uuid;
        _createdResourceIds.Add(resourceId);

        // Retrieve the resource with specific properties
        var getResponse = await _client.Resources.GetResourceByIdAsync(
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

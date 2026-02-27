using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia;
using Progress.Nuclia.Model;
using Xunit;

/// <summary>
/// Functional test to verify that the Nuclia SDK can modify an existing resource by its ID.
/// This test ensures that the SDK is able to connect to the real API using provided credentials and update resource properties using ModifyResourceByIdAsync.
/// Expected result: The API call succeeds and returns a ResourceUpdated response confirming the modification.
/// </summary>
public class ModifyResourceByIdFunctionalTest : IAsyncLifetime
{
    private readonly NucliaDbClient _client;
    private readonly string _testPrefix;
    private readonly List<string> _createdResourceIds = new();

    /// <summary>
    /// Initializes the test client using environment variables for credentials.
    /// Throws if required credentials are missing.
    /// </summary>
    public ModifyResourceByIdFunctionalTest()
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
                // Ignore cleanup errors to prevent test failures
            }
        }
    }

    /// <summary>
    /// Verifies that the SDK can modify an existing resource using ModifyResourceByIdAsync.
    /// The test passes if the API call succeeds and returns a valid ResourceUpdated response.
    /// </summary>
    [Fact(DisplayName = "Can modify resource by ID using ModifyResourceByIdAsync")]
    [Trait("Category", "Functional")]
    public async Task CanModifyResourceById()
    {
        // First, create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{_testPrefix}-modify-test",
            Summary = "Original summary for modification test",
            Icon = "application/stf-link"
        };

        var createResponse = await _client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        Assert.False(string.IsNullOrEmpty(createResponse.Data.Uuid), "Created resource UUID is null or empty");

        var resourceId = createResponse.Data.Uuid;
        _createdResourceIds.Add(resourceId);

        // Now modify the resource
        var updatePayload = new UpdateResourcePayload
        {
            Title = $"{_testPrefix}-modified-title",
            Summary = "Updated summary after modification"
        };

        var modifyResponse = await _client.Resources.ModifyResourceByIdAsync(resourceId, updatePayload);

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
    public async Task CanModifyResourceAndAddTextField()
    {
        // First, create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{_testPrefix}-modify-text-test",
            Summary = "Resource for testing text field modification",
            Icon = "application/stf-link"
        };

        var createResponse = await _client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        _createdResourceIds.Add(resourceId);

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

        var modifyResponse = await _client.Resources.ModifyResourceByIdAsync(resourceId, updatePayload);

        // Verify the modification response
        Assert.NotNull(modifyResponse);
        Assert.True(modifyResponse.Success, $"ModifyResourceById API call failed: {modifyResponse.Error}");
        Assert.NotNull(modifyResponse.Data);

        // Optionally verify the modification by fetching the resource
        var getResponse = await _client.Resources.GetResourceByIdAsync(resourceId);
        Assert.NotNull(getResponse);
        Assert.True(getResponse.Success, $"GetResourceById failed: {getResponse.Error}");
        Assert.NotNull(getResponse.Data);
    }
}

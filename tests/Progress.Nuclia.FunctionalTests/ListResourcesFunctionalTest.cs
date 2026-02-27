using System;
using System.Threading.Tasks;
using Xunit;
using Progress.Nuclia;

/// <summary>
/// Functional test to verify that the Nuclia SDK can list resources in the configured test Knowledge Box.
/// This test ensures that the SDK is able to connect to the real API using provided credentials and retrieve a list of resources.
/// Expected result: The API call succeeds and returns a valid resource list.
/// </summary>
public class ListResourcesFunctionalTest
{
    private readonly NucliaDbClient _client;

    /// <summary>
    /// Initializes the test client using environment variables for credentials.
    /// Throws if required credentials are missing.
    /// </summary>
    public ListResourcesFunctionalTest()
    {
        // Load credentials from environment variables or user secrets
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
    /// Verifies that the SDK can list resources in the test Knowledge Box.
    /// The test passes if the API call succeeds and returns a valid response.
    /// </summary>
    [Fact(DisplayName = "Can list resources in test KB")]
    [Trait("Category", "Functional")]
    public async Task CanListResources()
    {
        // List resources
        var response = await _client.Resources.ListResourcesAsync();
        Assert.NotNull(response);
        Assert.True(response.Success, $"API call failed: {response.Error}");
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Resources);
    }
}

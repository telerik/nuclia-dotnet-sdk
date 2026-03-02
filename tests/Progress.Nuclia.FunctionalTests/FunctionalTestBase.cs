using System;
using Progress.Nuclia;

namespace Progress.Nuclia.FunctionalTests;

/// <summary>
/// Base class for all functional tests. Handles client initialization and credential loading.
/// </summary>
public abstract class FunctionalTestBase
{
    /// <summary>
    /// The NucliaDbClient instance for interacting with the Nuclia API.
    /// </summary>
    protected readonly NucliaDbClient Client;

    /// <summary>
    /// Initializes the test client using environment variables for credentials.
    /// Throws if required credentials are missing.
    /// </summary>
    protected FunctionalTestBase()
    {
        // Load credentials from environment variables
        var zoneId = Environment.GetEnvironmentVariable("NUCLIA_ZONE_ID");
        var kbId = Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        var apiKey = Environment.GetEnvironmentVariable("NUCLIA_API_KEY");

        if (string.IsNullOrEmpty(zoneId) || string.IsNullOrEmpty(kbId) || string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException(
                "Missing Nuclia API credentials. Set NUCLIA_ZONE_ID, NUCLIA_KB_ID, NUCLIA_API_KEY.");
        }

        var config = new NucliaDbConfig(zoneId, kbId, apiKey);
        Client = new NucliaDbClient(config);
    }
}

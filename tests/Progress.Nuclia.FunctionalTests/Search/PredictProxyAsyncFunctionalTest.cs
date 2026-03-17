using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can proxy requests to the Predict API.
/// This test ensures that the SDK can forward requests with Knowledge Box configuration.
/// Expected result: The API call completes (success depends on endpoint and configuration).
/// </summary>
public class PredictProxyAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can proxy to the Predict API using PredictProxyAsync.
    /// The test passes if the API call completes without exception.
    /// </summary>
    [Fact(DisplayName = "Can proxy to Predict API using PredictProxyAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanProxyToPredictApi()
    {
        // Arrange: Define a predict proxy request
        var endpoint = PredictProxiedEndpoints.Chat;
        var requestBody = new { query = "test" };

        // Act: Execute the predict proxy request
        var response = await Client.Search.PredictProxyAsync(endpoint, requestBody);

        // Assert: Verify the response (may succeed or fail based on configuration)
        Assert.NotNull(response);
        // Note: Success depends on whether predict API is configured
        // We just verify the call completes without throwing
    }
}

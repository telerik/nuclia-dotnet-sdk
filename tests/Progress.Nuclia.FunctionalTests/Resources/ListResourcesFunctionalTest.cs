using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to verify that the Nuclia SDK can list resources in the configured test Knowledge Box.
/// This test ensures that the SDK is able to connect to the real API using provided credentials and retrieve a list of resources.
/// Expected result: The API call succeeds and returns a valid resource list.
/// </summary>
public class ListResourcesFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can list resources in the test Knowledge Box.
    /// The test passes if the API call succeeds and returns a valid response.
    /// </summary>
    [Fact(DisplayName = "Can list resources in test KB")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanListResources()
    {
        // List resources
        var response = await Client.Resources.ListResourcesAsync();
        Assert.NotNull(response);
        Assert.True(response.Success, $"API call failed: {response.Error}");
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Resources);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.KnowledgeBox;

/// <summary>
/// Functional test to verify that the Nuclia SDK can retrieve Knowledge Box configuration.
/// This test ensures that the SDK can fetch the models configuration dictionary.
/// Expected result: The API call succeeds and returns configuration data.
/// </summary>
public class GetKnowledgeBoxConfigurationAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can get Knowledge Box configuration.
    /// The test passes if the API call succeeds and returns configuration dictionary.
    /// </summary>
    [Fact(DisplayName = "Can get Knowledge Box configuration using GetKnowledgeBoxConfigurationAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "KnowledgeBoxService")]
    public async Task CanGetKnowledgeBoxConfiguration()
    {
        // Arrange: Get the KB ID from environment
        var kbId = System.Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        Assert.False(string.IsNullOrEmpty(kbId), "NUCLIA_KB_ID must be set");

        // Act: Get the Knowledge Box configuration
        var response = await Client.KnowledgeBoxes.GetKnowledgeBoxConfigurationAsync(kbId);

        // Assert: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Get Knowledge Box configuration failed: {response.Error}");
        Assert.NotNull(response.Data);
        Assert.IsType<Dictionary<string, object>>(response.Data);
    }
}

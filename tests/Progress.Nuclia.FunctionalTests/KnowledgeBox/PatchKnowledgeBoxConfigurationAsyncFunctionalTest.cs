using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.KnowledgeBox;

/// <summary>
/// Functional test to verify that the Nuclia SDK can patch Knowledge Box models configuration.
/// This test ensures that the SDK can perform partial updates to KB models configuration 
/// (such as semantic models, generative models, and other AI model settings).
/// Expected result: The API call succeeds and configuration is updated.
/// </summary>
public class PatchKnowledgeBoxConfigurationAsyncFunctionalTest : FunctionalTestBase, IAsyncLifetime
{
    private Dictionary<string, object>? _originalConfig;

    /// <summary>
    /// Save the original configuration before tests.
    /// </summary>
    public async Task InitializeAsync()
    {
        var kbId = System.Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        if (!string.IsNullOrEmpty(kbId))
        {
            var response = await Client.KnowledgeBoxes.GetKnowledgeBoxConfigurationAsync(kbId);
            if (response.Success)
            {
                _originalConfig = response.Data;
            }
        }
    }

    /// <summary>
    /// Restore the original configuration after tests.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_originalConfig != null)
        {
            var kbId = System.Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
            if (!string.IsNullOrEmpty(kbId))
            {
                try
                {
                    await Client.KnowledgeBoxes.SetKnowledgeBoxConfigurationAsync(kbId, _originalConfig);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }

    /// <summary>
    /// Verifies that the SDK can patch Knowledge Box configuration.
    /// The test passes if the API call succeeds and returns true.
    /// </summary>
    [Fact(DisplayName = "Can patch Knowledge Box configuration using PatchKnowledgeBoxConfigurationAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "KnowledgeBoxService")]
    public async Task CanPatchKnowledgeBoxConfiguration()
    {
        // Arrange: Get the KB ID and create a patch
        var kbId = System.Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        Assert.False(string.IsNullOrEmpty(kbId), "NUCLIA_KB_ID must be set");

        var configPatch = new Dictionary<string, object>
        {
            // Patch a models configuration field (this is a models config endpoint, not general KB config)
            { "prefer_markdown_generative_response", true }
        };

        // Act: Patch the Knowledge Box configuration
        var response = await Client.KnowledgeBoxes.PatchKnowledgeBoxConfigurationAsync(kbId, configPatch);

        // Assert: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Patch Knowledge Box configuration failed: {response.Error}");
        Assert.True(response.Data, "Patch should return true on success");
    }
}

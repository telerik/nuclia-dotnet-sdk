using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.KnowledgeBox;

/// <summary>
/// Functional test to verify that the Nuclia SDK can set (replace) Knowledge Box configuration.
/// This test ensures that the SDK can update/replace the entire configuration for a Knowledge Box.
/// Expected result: The API call completes successfully.
/// </summary>
public class SetKnowledgeBoxConfigurationAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can set/replace Knowledge Box configuration.
    /// The test passes if the API call succeeds.
    /// </summary>
    [Fact(DisplayName = "Can set Knowledge Box configuration using SetKnowledgeBoxConfigurationAsync", Skip = "Test temporarily disabled")]
    [Trait("Category", "Functional")]
    [Trait("Service", "KnowledgeBoxService")]
    public async Task CanSetKnowledgeBoxConfiguration()
    {
        // Step 1: Create configuration dictionary
        var configuration = new Dictionary<string, object>
        {
            ["description"] = "Updated via SDK test"
        };

        // Step 2: Set configuration
        var response = await Client.KnowledgeBoxes.SetKnowledgeBoxConfigurationAsync(KnowledgeBoxId, configuration);
        
        // Step 3: Verify response
        Assert.True(response.Success, $"Set configuration failed: {response.Error}");
    }
}

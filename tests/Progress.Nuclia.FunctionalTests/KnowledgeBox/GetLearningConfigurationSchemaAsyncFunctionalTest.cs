using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.KnowledgeBox;

/// <summary>
/// Functional test to verify that the Nuclia SDK can get learning configuration schema.
/// This test ensures that the SDK can retrieve the JSON schema for learning configuration.
/// Expected result: The API call succeeds and returns the schema dictionary.
/// </summary>
public class GetLearningConfigurationSchemaAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can get learning configuration schema.
    /// The test passes if the API call succeeds and returns schema data.
    /// </summary>
    [Fact(DisplayName = "Can get learning configuration schema using GetLearningConfigurationSchemaAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "KnowledgeBoxService")]
    public async Task CanGetLearningConfigurationSchema()
    {
        // Act: Get the learning configuration schema
        var response = await Client.KnowledgeBoxes.GetLearningConfigurationSchemaAsync();

        // Assert: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Get learning configuration schema failed: {response.Error}");
        Assert.NotNull(response.Data);
        Assert.IsType<Dictionary<string, object>>(response.Data);
    }
}

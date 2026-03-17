using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.KnowledgeBox;

/// <summary>
/// Functional test to verify that the Nuclia SDK can retrieve Knowledge Box information by ID.
/// This test ensures that the SDK can fetch KB details using the KB identifier.
/// Expected result: The API call succeeds and returns KB information.
/// </summary>
public class GetKnowledgeBoxAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can get Knowledge Box information by ID.
    /// The test passes if the API call succeeds and returns KB data.
    /// </summary>
    [Fact(DisplayName = "Can get Knowledge Box by ID using GetKnowledgeBoxAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "KnowledgeBoxService")]
    public async Task CanGetKnowledgeBoxById()
    {
        // Arrange: Get the KB ID from environment
        var kbId = System.Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        Assert.False(string.IsNullOrEmpty(kbId), "NUCLIA_KB_ID must be set");

        // Act: Get the Knowledge Box information
        var response = await Client.KnowledgeBoxes.GetKnowledgeBoxAsync(kbId);

        // Assert: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Get Knowledge Box failed: {response.Error}");
        Assert.NotNull(response.Data);
        Assert.Equal(kbId, response.Data.Uuid);
    }
}

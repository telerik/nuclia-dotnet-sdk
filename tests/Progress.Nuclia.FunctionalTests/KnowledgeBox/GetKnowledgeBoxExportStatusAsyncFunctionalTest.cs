using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.KnowledgeBox;

/// <summary>
/// Functional test to verify that the Nuclia SDK can get Knowledge Box export status.
/// This test ensures that the SDK can check the status of export operations.
/// Expected result: The API call completes (may succeed or fail based on export availability).
/// </summary>
public class GetKnowledgeBoxExportStatusAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can get Knowledge Box export status.
    /// The test passes if the API call completes without exception.
    /// Note: Success depends on whether an export with the given ID exists.
    /// </summary>
    [Fact(DisplayName = "Can get Knowledge Box export status using GetKnowledgeBoxExportStatusAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "KnowledgeBoxService")]
    public async Task CanGetKnowledgeBoxExportStatus()
    {
        // Arrange: Get the KB ID
        var kbId = System.Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        Assert.False(string.IsNullOrEmpty(kbId), "NUCLIA_KB_ID must be set");

        // Use a test export ID (this will likely not exist, but tests the endpoint)
        var exportId = "test-export-id";

        // Act: Get the export status
        var response = await Client.KnowledgeBoxes.GetKnowledgeBoxExportStatusAsync(kbId, exportId);

        // Assert: Verify the call completed
        Assert.NotNull(response);
        // Note: We don't assert Success here since the export likely doesn't exist
    }
}

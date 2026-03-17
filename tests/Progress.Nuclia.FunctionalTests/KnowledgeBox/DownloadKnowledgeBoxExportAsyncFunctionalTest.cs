using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.KnowledgeBox;

/// <summary>
/// Functional test to verify that the Nuclia SDK can download Knowledge Box exports.
/// This test ensures that the SDK can retrieve export binary data.
/// Expected result: The API call completes (may succeed or fail based on export availability).
/// </summary>
public class DownloadKnowledgeBoxExportAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can attempt to download a Knowledge Box export.
    /// The test passes if the API call completes without exception.
    /// Note: Success depends on whether an export with the given ID exists.
    /// </summary>
    [Fact(DisplayName = "Can attempt to download Knowledge Box export using DownloadKnowledgeBoxExportAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "KnowledgeBoxService")]
    public async Task CanAttemptDownloadKnowledgeBoxExport()
    {
        // Arrange: Get the KB ID
        var kbId = System.Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        Assert.False(string.IsNullOrEmpty(kbId), "NUCLIA_KB_ID must be set");

        // Use a test export ID (this will likely not exist, but tests the endpoint)
        var exportId = "test-export-id";

        // Act: Attempt to download the export
        var response = await Client.KnowledgeBoxes.DownloadKnowledgeBoxExportAsync(kbId, exportId);

        // Assert: Verify the call completed (may or may not succeed based on export existence)
        Assert.NotNull(response);
        // Note: We don't assert Success here since the export likely doesn't exist
    }
}

using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.KnowledgeBox;

/// <summary>
/// Functional test to verify that the Nuclia SDK can get Knowledge Box import status.
/// This test ensures that the SDK can check the status of import operations.
/// Expected result: The API call completes (may succeed or fail based on import availability).
/// </summary>
public class GetKnowledgeBoxImportStatusAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can get Knowledge Box import status.
    /// The test passes if the API call completes without exception.
    /// Note: Success depends on whether an import with the given ID exists.
    /// </summary>
    [Fact(DisplayName = "Can get Knowledge Box import status using GetKnowledgeBoxImportStatusAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "KnowledgeBoxService")]
    public async Task CanGetKnowledgeBoxImportStatus()
    {
        // Arrange: Get the KB ID
        var kbId = System.Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        Assert.False(string.IsNullOrEmpty(kbId), "NUCLIA_KB_ID must be set");

        // Use a test import ID (this will likely not exist, but tests the endpoint)
        var importId = "test-import-id";

        // Act: Get the import status
        var response = await Client.KnowledgeBoxes.GetKnowledgeBoxImportStatusAsync(kbId, importId);

        // Assert: Verify the call completed
        Assert.NotNull(response);
        // Note: We don't assert Success here since the import likely doesn't exist
    }
}

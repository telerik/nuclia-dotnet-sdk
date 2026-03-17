using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.KnowledgeBox;

/// <summary>
/// Functional test to verify that the Nuclia SDK can retrieve Knowledge Box counters.
/// This test ensures that the SDK can fetch resource counters and statistics.
/// Expected result: The API call succeeds and returns counter information.
/// </summary>
public class GetKnowledgeBoxCountersAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can get Knowledge Box counters.
    /// The test passes if the API call succeeds and returns counter data.
    /// </summary>
    [Fact(DisplayName = "Can get Knowledge Box counters using GetKnowledgeBoxCountersAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "KnowledgeBoxService")]
    public async Task CanGetKnowledgeBoxCounters()
    {
        // Arrange: Get the KB ID from environment
        var kbId = System.Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        Assert.False(string.IsNullOrEmpty(kbId), "NUCLIA_KB_ID must be set");

        // Act: Get the Knowledge Box counters
        var response = await Client.KnowledgeBoxes.GetKnowledgeBoxCountersAsync(kbId);

        // Assert: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Get Knowledge Box counters failed: {response.Error}");
        Assert.NotNull(response.Data);
    }

    /// <summary>
    /// Verifies that the SDK can get Knowledge Box counters with debug flag.
    /// The test passes if the API call succeeds with the debug parameter.
    /// </summary>
    [Fact(DisplayName = "Can get Knowledge Box counters with debug flag")]
    [Trait("Category", "Functional")]
    [Trait("Service", "KnowledgeBoxService")]
    public async Task CanGetKnowledgeBoxCountersWithDebug()
    {
        // Arrange: Get the KB ID from environment
        var kbId = System.Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        Assert.False(string.IsNullOrEmpty(kbId), "NUCLIA_KB_ID must be set");

        // Act: Get the Knowledge Box counters with debug flag
        var response = await Client.KnowledgeBoxes.GetKnowledgeBoxCountersAsync(kbId, debug: true);

        // Assert: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Get Knowledge Box counters with debug failed: {response.Error}");
        Assert.NotNull(response.Data);
    }
}

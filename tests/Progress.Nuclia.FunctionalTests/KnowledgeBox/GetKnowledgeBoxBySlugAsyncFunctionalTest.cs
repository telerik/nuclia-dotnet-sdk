using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.KnowledgeBox;

/// <summary>
/// Functional test to verify that the Nuclia SDK can retrieve Knowledge Box information by slug.
/// This test ensures that the SDK can fetch KB details using the slug identifier.
/// Expected result: The API call succeeds and returns KB information.
/// </summary>
public class GetKnowledgeBoxBySlugAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can get Knowledge Box information by slug.
    /// The test passes if the API call succeeds and returns KB data.
    /// </summary>
    [Fact(DisplayName = "Can get Knowledge Box by slug using GetKnowledgeBoxBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "KnowledgeBoxService")]
    public async Task CanGetKnowledgeBoxBySlug()
    {
        // Arrange: Get the KB ID first to retrieve its slug
        var kbId = System.Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        Assert.False(string.IsNullOrEmpty(kbId), "NUCLIA_KB_ID must be set");

        var kbResponse = await Client.KnowledgeBoxes.GetKnowledgeBoxAsync(kbId);
        Assert.True(kbResponse.Success, "Failed to get KB info for slug lookup");
        Assert.NotNull(kbResponse.Data?.Slug);
        
        var slug = kbResponse.Data.Slug;

        // Act: Get the Knowledge Box by slug
        var response = await Client.KnowledgeBoxes.GetKnowledgeBoxBySlugAsync(slug);

        // Assert: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Get Knowledge Box by slug failed: {response.Error}");
        Assert.NotNull(response.Data);
        Assert.Equal(slug, response.Data.Slug);
    }
}

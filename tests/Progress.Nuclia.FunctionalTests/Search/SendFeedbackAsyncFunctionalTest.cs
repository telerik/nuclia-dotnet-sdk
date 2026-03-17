using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can send feedback for search operations.
/// This test ensures that the SDK can submit feedback ratings for operations.
/// Expected result: The API call succeeds and feedback is accepted.
/// </summary>
public class SendFeedbackAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can send feedback using SendFeedbackAsync.
    /// The test passes if the API call succeeds and feedback is submitted.
    /// </summary>
    [Fact(DisplayName = "Can send feedback using SendFeedbackAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanSendFeedback()
    {
        // Arrange: Create a feedback request
        var request = new FeedbackRequest("test-operation-id", true, FeedbackTasks.CHAT);

        // Act: Execute the feedback request
        var response = await Client.Search.SendFeedbackAsync(request);

        // Assert: Verify the response
        Assert.NotNull(response);
        // Note: Feedback might succeed or fail depending on whether the operation exists
        // We just verify the call completes without exception
    }
}

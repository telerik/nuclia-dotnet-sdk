using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can ask questions to a specific resource.
/// This test ensures that the SDK can perform resource-scoped Ask operations.
/// Expected result: The API call succeeds and returns an answer based on the resource.
/// </summary>
public class AskResourceAsyncFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can ask a question to a specific resource using AskResourceAsync.
    /// The test passes if the API call succeeds and returns a response.
    /// </summary>
    [Fact(DisplayName = "Can ask a question to a resource using AskResourceAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanAskResourceQuestion()
    {
        // Step 1: Create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-ask-resource-test",
            Summary = "Test resource for resource-scoped Ask functionality with searchable content about artificial intelligence",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Ask a question to the resource
        var askRequest = new AskRequest("What is this resource about?");

        var response = await Client.Search.AskResourceAsync(resourceId, askRequest);

        // Step 3: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Ask resource request failed: {response.Error}");
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Answer);
    }
}

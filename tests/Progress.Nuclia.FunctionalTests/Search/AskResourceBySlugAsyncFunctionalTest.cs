using System;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can ask questions to a specific resource by slug.
/// This test ensures that the SDK can perform resource-scoped Ask operations using slug identifier.
/// Expected result: The API call succeeds and returns an answer based on the resource.
/// </summary>
public class AskResourceBySlugAsyncFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can ask a question to a resource by slug using AskResourceBySlugAsync.
    /// The test passes if the API call succeeds and returns a response.
    /// </summary>
    [Fact(DisplayName = "Can ask a question to a resource by slug using AskResourceBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanAskResourceQuestionBySlug()
    {
        // Step 1: Create a test resource with a slug
        var testSlug = $"test-slug-ask-{Guid.NewGuid()}";
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-ask-by-slug-test",
            Summary = "Test resource for Ask by slug with information about deep learning",
            Slug = testSlug,
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Ask a question to the resource by slug
        var askRequest = new AskRequest("What is this resource about?");

        var response = await Client.Search.AskResourceBySlugAsync(testSlug, askRequest);

        // Step 3: Verify the response
        Assert.NotNull(response);
        Assert.True(response.Success, $"Ask resource by slug request failed: {response.Error}");
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Answer);
    }
}

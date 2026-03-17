using System;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Search;

/// <summary>
/// Functional test to verify that the Nuclia SDK can ask questions to a resource by slug with streaming.
/// This test ensures that the SDK can perform resource-scoped streaming Ask operations using slug identifier.
/// Expected result: The API call succeeds and returns streaming response chunks.
/// </summary>
public class AskResourceBySlugStreamAsyncFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can ask a question to a resource by slug with streaming.
    /// The test passes if streaming chunks are received successfully.
    /// </summary>
    [Fact(DisplayName = "Can ask a question to a resource by slug with streaming")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanAskResourceBySlugWithStreaming()
    {
        // Step 1: Create a test resource with a slug
        var testSlug = $"test-slug-stream-{Guid.NewGuid()}";
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-ask-slug-stream-test",
            Summary = "Test resource for Ask by slug with streaming, containing information about transformers",
            Slug = testSlug,
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Ask with streaming by slug
        var askRequest = new AskRequest("Summarize this resource");

        var chunks = new System.Collections.Generic.List<Model.Streaming.SyncAskResponseUpdate>();
        await foreach (var chunk in Client.Search.AskResourceBySlugStreamAsync(testSlug, askRequest))
        {
            chunks.Add(chunk);
        }

        // Step 3: Verify streaming chunks received
        Assert.NotEmpty(chunks);
    }

    /// <summary>
    /// Verifies streaming Ask by slug with string query overload.
    /// The test passes if streaming chunks are received successfully.
    /// </summary>
    [Fact(DisplayName = "Can ask resource by slug with string query and streaming")]
    [Trait("Category", "Functional")]
    [Trait("Service", "SearchService")]
    public async Task CanAskResourceBySlugWithStringQueryStreaming()
    {
        // Step 1: Create a test resource with a slug
        var testSlug = $"test-slug-string-{Guid.NewGuid()}";
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-ask-slug-string-test",
            Summary = "Test resource for string query streaming by slug",
            Slug = testSlug,
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Ask with string query and streaming
        var chunks = new System.Collections.Generic.List<Model.Streaming.SyncAskResponseUpdate>();
        await foreach (var chunk in Client.Search.AskResourceBySlugStreamAsync(testSlug, "What is this?"))
        {
            chunks.Add(chunk);
        }

        // Step 3: Verify chunks received
        Assert.NotEmpty(chunks);
    }
}

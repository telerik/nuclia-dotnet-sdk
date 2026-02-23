using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Progress.Nuclia;
using Progress.Nuclia.Model;
using Progress.Nuclia.Model.Streaming;
using Xunit;

namespace Progress.Nuclia.Tests;

internal class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
    public List<HttpRequestMessage> CapturedRequests { get; } = new();

    public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder;
    }   

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        CapturedRequests.Add(request);
        return Task.FromResult(_responder(request));
    }
}

public class ResourceServiceTests
{
    private NucliaDbClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> responder, out StubHttpMessageHandler handler)
    {
        handler = new StubHttpMessageHandler(responder);
        var httpClient = new HttpClient(handler);
        var config = new NucliaDbConfig("test-zone", "kb123", "dummy-key");
        return new NucliaDbClient(httpClient, config);
    }

    [Fact]
    public async Task ReindexResourceByIdAsync_ReturnsSuccessOn204()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.NoContent), out _);
        var response = await client.Resources.ReindexResourceByIdAsync("rid123", true);
        Assert.True(response.Success);
        Assert.True(response.Data);
    }

    [Fact]
    public async Task ReindexResourceByIdAsync_IncludesQueryParamWhenReindexVectorsTrue()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.NoContent), out var handler);
        var _ = await client.Resources.ReindexResourceByIdAsync("rid456", true);
        Assert.Single(handler.CapturedRequests);
        var uri = handler.CapturedRequests[0].RequestUri!;
        Assert.Contains("reindex_vectors=true", uri.Query);
    }

    [Fact]
    public async Task ReprocessResourceByIdAsync_ParsesSeqid()
    {
        var json = "{\"seqid\":123}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.Accepted)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        }, out _);
        var response = await client.Resources.ReprocessResourceByIdAsync("rid789", resetTitle: true);
        Assert.True(response.Success);
        Assert.Equal(123, response.Data!.Seqid);
    }

    [Fact]
    public async Task RunAgentsOnResourceByIdAsync_ParsesResults()
    {
        var json = "{\"results\":{\"field1\":{\"metadata\":{\"links\":[],\"paragraphs\":[],\"ner\":{},\"entities\":{},\"classifications\":[],\"positions\":{}},\"applied_data_augmentation\":{\"new_text_fields\":[],\"changed\":false},\"input_nuclia_tokens\":0,\"output_nuclia_tokens\":0,\"time\":0}}}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        }, out var handler);
        var request = new ResourceAgentsRequest();
        var response = await client.Resources.RunAgentsOnResourceByIdAsync("rid999", request);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Contains("field1", response.Data!.Results.Keys);
        // Verify request was issued
        Assert.Single(handler.CapturedRequests);
    }

    [Fact]
    public async Task SearchWithinResourceByIdAsync_BuildsQueryString()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        }, out var handler);
        var response = await client.Resources.SearchWithinResourceByIdAsync("ridSearch", "hello", fields: new[] { "a/title" }, topK: 10, highlight: true);
        Assert.True(response.Success);
        var uri = handler.CapturedRequests[0].RequestUri!;
        Assert.Contains("query=hello", uri.Query);
        Assert.Contains("fields=", uri.Query); // value encoding may vary
        Assert.Contains("top_k=10", uri.Query);
        Assert.Contains("highlight=True", uri.Query);
    }

    [Fact]
    public async Task SearchWithinResourceByIdAsync_ReturnsErrorWhenQueryMissing()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        }, out _);
        var response = await client.Resources.SearchWithinResourceByIdAsync("ridSearch", "");
        Assert.False(response.Success);
        Assert.NotNull(response.Error);
        Assert.Null(response.Data);
    }

    [Fact]
    public async Task ModifyResourceBySlugAsync_SendsRequestToCorrectEndpoint()
    {
        var responseJson = "{\"seqid\": 123}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);
        
        var payload = new UpdateResourcePayload
        {
            Title = "Updated Title",
            Summary = "Updated summary"
        };
        
        var response = await client.Resources.ModifyResourceBySlugAsync("resource-slug", payload);
        
        Assert.True(response.Success);
        Assert.Single(handler.CapturedRequests);
        var httpReq = handler.CapturedRequests[0];
        Assert.Equal(HttpMethod.Patch, httpReq.Method);
        Assert.Contains("/kb/kb123/slug/resource-slug", httpReq.RequestUri!.ToString());
        Assert.NotNull(httpReq.Content);
    }

    [Fact]
    public async Task ModifyResourceBySlugAsync_WithSkipStore_AddsCorrectHeader()
    {
        var responseJson = "{\"seqid\": 456}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);
        
        var payload = new UpdateResourcePayload
        {
            Title = "Test Title"
        };
        
        var response = await client.Resources.ModifyResourceBySlugAsync("test-slug", payload, skipStore: true);
        
        Assert.True(response.Success);
        Assert.True(handler.CapturedRequests[0].Headers.Contains("x-skip-store"));
        var skipStoreValues = handler.CapturedRequests[0].Headers.GetValues("x-skip-store");
        Assert.Contains("true", skipStoreValues);
    }

    [Fact]
    public async Task ModifyResourceBySlugAsync_WithoutSkipStore_DoesNotAddHeader()
    {
        var responseJson = "{\"seqid\": 789}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);
        
        var payload = new UpdateResourcePayload
        {
            Summary = "Test Summary"
        };
        
        var response = await client.Resources.ModifyResourceBySlugAsync("another-slug", payload);
        
        Assert.True(response.Success);
        Assert.False(handler.CapturedRequests[0].Headers.Contains("x-skip-store"));
    }

    [Fact]
    public async Task ModifyResourceBySlugAsync_ValidationError_ReturnsValidationError()
    {
        var validationErrorJson = "{\"detail\":[{\"loc\":[\"body\",\"title\"],\"msg\":\"Field required\",\"type\":\"missing\"}]}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
        {
            Content = new StringContent(validationErrorJson, Encoding.UTF8, "application/json")
        }, out _);
        
        var payload = new UpdateResourcePayload(); // Empty payload might cause validation error
        var response = await client.Resources.ModifyResourceBySlugAsync("test-slug", payload);
        
        Assert.False(response.Success);
        Assert.NotNull(response.ValidationError);
    }

    [Fact]
    public async Task ModifyResourceBySlugAsync_HttpError_ReturnsError()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Resource not found", Encoding.UTF8, "text/plain")
        }, out _);
        
        var payload = new UpdateResourcePayload
        {
            Title = "Test Title"
        };
        var response = await client.Resources.ModifyResourceBySlugAsync("nonexistent-slug", payload);
        
        Assert.False(response.Success);
        Assert.Contains("HTTP request failed", response.Error);
    }

    [Fact]
    public async Task ModifyResourceByIdAsync_SendsRequestToCorrectEndpoint()
    {
        var responseJson = "{\"seqid\": 123}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);
        
        var payload = new UpdateResourcePayload
        {
            Title = "Updated Title",
            Summary = "Updated summary"
        };
        
        var response = await client.Resources.ModifyResourceByIdAsync("resource-id-123", payload);
        
        Assert.True(response.Success);
        Assert.Single(handler.CapturedRequests);
        var httpReq = handler.CapturedRequests[0];
        Assert.Equal(HttpMethod.Patch, httpReq.Method);
        Assert.Contains("/kb/kb123/resource/resource-id-123", httpReq.RequestUri!.ToString());
        Assert.NotNull(httpReq.Content);
    }

    [Fact]
    public async Task ModifyResourceByIdAsync_WithSkipStore_AddsCorrectHeader()
    {
        var responseJson = "{\"seqid\": 456}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);
        
        var payload = new UpdateResourcePayload
        {
            Title = "Test Title"
        };
        
        var response = await client.Resources.ModifyResourceByIdAsync("test-id", payload, skipStore: true);
        
        Assert.True(response.Success);
        Assert.True(handler.CapturedRequests[0].Headers.Contains("x-skip-store"));
        var skipStoreValues = handler.CapturedRequests[0].Headers.GetValues("x-skip-store");
        Assert.Contains("true", skipStoreValues);
    }

    [Fact]
    public async Task ModifyResourceByIdAsync_WithoutSkipStore_DoesNotAddHeader()
    {
        var responseJson = "{\"seqid\": 789}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);
        
        var payload = new UpdateResourcePayload
        {
            Summary = "Test Summary"
        };
        
        var response = await client.Resources.ModifyResourceByIdAsync("another-id", payload);
        
        Assert.True(response.Success);
        Assert.False(handler.CapturedRequests[0].Headers.Contains("x-skip-store"));
    }

    [Fact]
    public async Task ModifyResourceByIdAsync_ValidationError_ReturnsValidationError()
    {
        var validationErrorJson = "{\"detail\":[{\"loc\":[\"body\",\"title\"],\"msg\":\"Field required\",\"type\":\"missing\"}]}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
        {
            Content = new StringContent(validationErrorJson, Encoding.UTF8, "application/json")
        }, out _);
        
        var payload = new UpdateResourcePayload(); // Empty payload might cause validation error
        var response = await client.Resources.ModifyResourceByIdAsync("test-id", payload);
        
        Assert.False(response.Success);
        Assert.NotNull(response.ValidationError);
    }

    [Fact]
    public async Task ModifyResourceByIdAsync_HttpError_ReturnsError()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Resource not found", Encoding.UTF8, "text/plain")
        }, out _);
        
        var payload = new UpdateResourcePayload
        {
            Title = "Test Title"
        };
        var response = await client.Resources.ModifyResourceByIdAsync("nonexistent-id", payload);
        
        Assert.False(response.Success);
        Assert.Contains("HTTP request failed", response.Error);
    }
}

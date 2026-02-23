using Progress.Nuclia.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.Tests;

public class KnowledgeBoxServiceTests
{
    private NucliaDbClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> responder, out StubHttpMessageHandler handler)
    {
        handler = new StubHttpMessageHandler(responder);
        var httpClient = new HttpClient(handler);
        var config = new NucliaDbConfig("test-zone", "kb123", "dummy-key");
        return new NucliaDbClient(httpClient, config);
    }

    #region GetKnowledgeBoxCountersAsync Tests

    [Fact]
    public async Task GetKnowledgeBoxCountersAsync_ReturnsSuccess()
    {
        var json = "{\"resources\":100,\"paragraphs\":500,\"fields\":250,\"sentences\":1500,\"index_size\":10.5}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        }, out _);
        
        var response = await client.KnowledgeBoxes.GetKnowledgeBoxCountersAsync("kb123");
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(100, response.Data!.Resources);
        Assert.Equal(500, response.Data.Paragraphs);
        Assert.Equal(250, response.Data.Fields);
    }

    [Fact]
    public async Task GetKnowledgeBoxCountersAsync_IncludesDebugParam()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        }, out var handler);
        
        await client.KnowledgeBoxes.GetKnowledgeBoxCountersAsync("kb123", debug: true);
        
        Assert.Single(handler.CapturedRequests);
        var uri = handler.CapturedRequests[0].RequestUri!;
        Assert.Contains("debug=true", uri.Query);
    }

    #endregion

    #region Configuration Tests

    [Fact]
    public async Task GetKnowledgeBoxConfigurationAsync_ReturnsConfiguration()
    {
        var configJson = "{\"max_resources\":1000,\"similarity_threshold\":0.7,\"custom_setting\":\"value\"}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(configJson, Encoding.UTF8, "application/json")
        }, out _);
        
        var response = await client.KnowledgeBoxes.GetKnowledgeBoxConfigurationAsync("kb123");
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.True(response.Data!.ContainsKey("max_resources"));
        Assert.Equal(1000L, ((JsonElement)response.Data["max_resources"]).GetInt64());
    }

    [Fact]
    public async Task PatchKnowledgeBoxConfigurationAsync_ReturnsSuccessOn204()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.NoContent), out var handler);
        var configPatch = new Dictionary<string, object> { ["max_resources"] = 2000 };
        
        var response = await client.KnowledgeBoxes.PatchKnowledgeBoxConfigurationAsync("kb123", configPatch);
        
        Assert.True(response.Success);
        Assert.True(response.Data);
        Assert.Single(handler.CapturedRequests);
        Assert.Equal(HttpMethod.Patch, handler.CapturedRequests[0].Method);
    }

    [Fact]
    public async Task SetKnowledgeBoxConfigurationAsync_ReturnsSuccessOn204()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.NoContent), out var handler);
        var config = new Dictionary<string, object> { 
            ["similarity_threshold"] = 0.8,
            ["custom_setting"] = "new_value"
        };
        
        var response = await client.KnowledgeBoxes.SetKnowledgeBoxConfigurationAsync("kb123", config);
        
        Assert.True(response.Success);
        Assert.True(response.Data);
        Assert.Single(handler.CapturedRequests);
        Assert.Equal(HttpMethod.Post, handler.CapturedRequests[0].Method);
    }

    [Fact]
    public async Task PatchKnowledgeBoxConfigurationAsync_HandlesError()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Bad request", Encoding.UTF8, "text/plain")
        }, out _);
        var configPatch = new Dictionary<string, object> { ["invalid"] = "value" };
        
        var response = await client.KnowledgeBoxes.PatchKnowledgeBoxConfigurationAsync("kb123", configPatch);
        
        Assert.False(response.Success);
        Assert.Contains("HTTP request failed", response.Error);
    }

    #endregion

    #region Export Tests

    [Fact]
    public async Task DownloadKnowledgeBoxExportAsync_ReturnsBinaryData()
    {
        var binaryData = new byte[] { 0x50, 0x4B, 0x03, 0x04 }; // ZIP file signature
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(binaryData)
        }, out _);
        
        var response = await client.KnowledgeBoxes.DownloadKnowledgeBoxExportAsync("kb123", "export456");
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(binaryData, response.Data);
    }

    [Fact]
    public async Task GetKnowledgeBoxExportStatusAsync_ReturnsStatus()
    {
        var statusJson = "{\"status\":{\"status\":\"PROCESSED\"},\"processed\":100,\"total\":100,\"retries\":0}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(statusJson, Encoding.UTF8, "application/json")
        }, out _);
        
        var response = await client.KnowledgeBoxes.GetKnowledgeBoxExportStatusAsync("kb123", "export456");
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(ResourceProcessingStatus.PROCESSED, response.Data!.Status.VarStatus);
        Assert.Equal(100, response.Data.Processed);
    }

    #endregion

    #region Import Tests

    [Fact]
    public async Task GetKnowledgeBoxImportStatusAsync_ReturnsStatus()
    {
        var statusJson = "{\"status\":{\"status\":\"PENDING\"},\"processed\":50,\"total\":100,\"retries\":2}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(statusJson, Encoding.UTF8, "application/json")
        }, out _);
        
        var response = await client.KnowledgeBoxes.GetKnowledgeBoxImportStatusAsync("kb123", "import789");
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(ResourceProcessingStatus.PENDING, response.Data!.Status.VarStatus);
        Assert.Equal(50, response.Data.Processed);
        Assert.Equal(2, response.Data.Retries);
    }

    #endregion

    #region Upload Tests

    [Fact]
    public async Task UploadKnowledgeBoxFileAsync_ReturnsCreatedResource()
    {
        var uploadJson = "{\"seqid\":123,\"uuid\":\"resource-uuid-456\",\"field_id\":\"file\"}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(uploadJson, Encoding.UTF8, "application/json")
        }, out var handler);
        
        var fileData = new MemoryStream(Encoding.UTF8.GetBytes("test file content"));
        var response = await client.KnowledgeBoxes.UploadKnowledgeBoxFileAsync("kb123", fileData, "test.txt");
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(123, response.Data!.Seqid);
        Assert.Equal("resource-uuid-456", response.Data.Uuid);
        Assert.Equal("file", response.Data.FieldId);
        
        // Verify request structure
        Assert.Single(handler.CapturedRequests);
        var request = handler.CapturedRequests[0];
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Contains("/kb/kb123/upload", request.RequestUri!.ToString());
    }

    [Fact]
    public async Task UploadKnowledgeBoxFileAsync_IncludesSplitStrategyHeader()
    {
        var client = CreateClient(req => 
        {
            // Verify x-split-strategy header is present
            var hasSplitStrategy = req.Content?.Headers?.Contains("x-split-strategy") == true;
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("{\"seqid\":123}", Encoding.UTF8, "application/json")
            };
        }, out var handler);
        
        var fileData = new MemoryStream(Encoding.UTF8.GetBytes("test"));
        var response = await client.KnowledgeBoxes.UploadKnowledgeBoxFileAsync("kb123", fileData, "test.txt", "paragraph");
        
        Assert.True(response.Success);
        
        // Verify multipart content with split strategy header
        var request = handler.CapturedRequests[0];
        var content = request.Content as MultipartFormDataContent;
        Assert.NotNull(content);
    }

    #endregion

    #region Learning Configuration Schema Tests

    [Fact]
    public async Task GetLearningConfigurationSchemaAsync_ReturnsSchema()
    {
        var schemaJson = "{\"type\":\"object\",\"properties\":{\"generative_model\":{\"enum\":[\"chatgpt\",\"generative-multilingual-2023\"]}}}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(schemaJson, Encoding.UTF8, "application/json")
        }, out var handler);
        
        var response = await client.KnowledgeBoxes.GetLearningConfigurationSchemaAsync();
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.True(response.Data!.ContainsKey("type"));
        Assert.Equal("object", ((JsonElement)response.Data["type"]).GetString());
        
        // Verify correct endpoint
        Assert.Single(handler.CapturedRequests);
        Assert.Contains("/learning/configuration/schema", handler.CapturedRequests[0].RequestUri!.ToString());
    }

    [Fact]
    public async Task GetLearningConfigurationSchemaAsync_HandlesError()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Server error", Encoding.UTF8, "text/plain")
        }, out _);
        
        var response = await client.KnowledgeBoxes.GetLearningConfigurationSchemaAsync();
        
        Assert.False(response.Success);
        Assert.Contains("HTTP request failed", response.Error);
    }

    #endregion
}
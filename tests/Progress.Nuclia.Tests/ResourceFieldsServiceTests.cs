using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Progress.Nuclia;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.Tests;

public class ResourceFieldsServiceTests
{
    private NucliaDbClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> responder, out StubHttpMessageHandler handler)
    {
        handler = new StubHttpMessageHandler(responder);
        var httpClient = new HttpClient(handler);
        var config = new NucliaDbConfig("test-zone", "kb123", "dummy-key");
        return new NucliaDbClient(httpClient, config);
    }

    // ===== Conversation Field Tests =====

    [Fact]
    public async Task AddConversationFieldByIdAsync_SendsCorrectRequest()
    {
        var responseJson = "{\"seqid\": 100}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);

        var conversationField = new InputConversationField
        {
            Messages = new List<InputMessage>
            {
                new InputMessage(
                    content: new InputMessageContent("Hello"),
                    ident: "msg1"
                )
            }
        };

        var response = await client.ResourceFields.AddConversationFieldByIdAsync(
            "resource-123",
            "chat-field",
            conversationField
        );

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(100, response.Data.Seqid);
        Assert.Single(handler.CapturedRequests);
        var httpReq = handler.CapturedRequests[0];
        Assert.Equal(HttpMethod.Put, httpReq.Method);
        Assert.Contains("/kb/kb123/resource/resource-123/conversation/chat-field", httpReq.RequestUri!.ToString());
    }

    [Fact]
    public async Task AddConversationFieldBySlugAsync_UsesSlugEndpoint()
    {
        var responseJson = "{\"seqid\": 101}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);

        var conversationField = new InputConversationField();

        var response = await client.ResourceFields.AddConversationFieldBySlugAsync(
            "my-resource-slug",
            "chat-field",
            conversationField
        );

        Assert.True(response.Success);
        var httpReq = handler.CapturedRequests[0];
        Assert.Contains("/kb/kb123/slug/my-resource-slug/conversation/chat-field", httpReq.RequestUri!.ToString());
    }

    [Fact]
    public async Task AppendMessagesToConversationFieldByIdAsync_SendsMessagesList()
    {
        var responseJson = "{\"seqid\": 12345}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);

        var messages = new List<InputMessage>
        {
            new InputMessage(
                content: new InputMessageContent("Question"),
                ident: "msg2"
            )
        };

        var response = await client.ResourceFields.AppendMessagesToConversationFieldByIdAsync(
            "resource-123",
            "chat-field",
            messages
        );

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(12345, response.Data.Seqid);
        Assert.Single(handler.CapturedRequests);
        var httpReq = handler.CapturedRequests[0];
        Assert.Equal(HttpMethod.Put, httpReq.Method);
        Assert.Contains("/kb/kb123/resource/resource-123/conversation/chat-field/messages", httpReq.RequestUri!.ToString());
    }

    [Fact]
    public async Task DownloadConversationAttachmentByIdAsync_ReturnsByteArray()
    {
        var binaryContent = new byte[] { 1, 2, 3, 4, 5 };
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(binaryContent)
        }, out var handler);

        var response = await client.ResourceFields.DownloadConversationAttachmentByIdAsync(
            "resource-123",
            "chat-field",
            "msg5",
            0
        );

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(binaryContent, response.Data);
        var httpReq = handler.CapturedRequests[0];
        Assert.Contains("/kb/kb123/resource/resource-123/conversation/chat-field/download/field/msg5/0", httpReq.RequestUri!.ToString());
    }

    // ===== File Field Tests =====

    [Fact]
    public async Task AddFileFieldByIdAsync_SendsCorrectRequest()
    {
        var responseJson = "{\"seqid\": 200}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);

        var fileField = new FileField(new File { Uri = "https://example.com/doc.pdf" });

        var response = await client.ResourceFields.AddFileFieldByIdAsync(
            "resource-123",
            "file-field",
            fileField
        );

        Assert.True(response.Success);
        Assert.Equal(200, response.Data!.Seqid);
        var httpReq = handler.CapturedRequests[0];
        Assert.Equal(HttpMethod.Put, httpReq.Method);
        Assert.Contains("/kb/kb123/resource/resource-123/file/file-field", httpReq.RequestUri!.ToString());
    }

    [Fact]
    public async Task AddFileFieldByIdAsync_WithSkipStore_AddsHeader()
    {
        var responseJson = "{\"seqid\": 201}";
        var client = CreateClient(req =>
        {
            Assert.True(req.Headers.Contains("x-skip-store"));
            Assert.Equal("true", req.Headers.GetValues("x-skip-store").First());
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
        }, out _);

        var fileField = new FileField(new File { Uri = "https://example.com/doc.pdf" });

        var response = await client.ResourceFields.AddFileFieldByIdAsync(
            "resource-123",
            "file-field",
            fileField,
            skipStore: true
        );

        Assert.True(response.Success);
    }

    [Fact]
    public async Task UploadFileByIdAsync_SendsBinaryContent()
    {
        var responseJson = "{\"seqid\": 300, \"uuid\": \"file-uuid-123\", \"field_id\": \"uploaded-file\"}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);

        var fileContent = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // PDF magic bytes

        var response = await client.ResourceFields.UploadFileByIdAsync(
            "resource-123",
            "pdf-field",
            fileContent,
            filename: "document.pdf",
            language: "en"
        );

        Assert.True(response.Success);
        Assert.Equal("file-uuid-123", response.Data!.Uuid);
        Assert.Equal("uploaded-file", response.Data!.FieldId);
        var httpReq = handler.CapturedRequests[0];
        Assert.Equal(HttpMethod.Post, httpReq.Method);
        Assert.Contains("/kb/kb123/resource/resource-123/file/pdf-field/upload", httpReq.RequestUri!.ToString());
        Assert.True(httpReq.Headers.Contains("x-filename"));
        Assert.True(httpReq.Headers.Contains("x-language"));
    }

    [Fact]
    public async Task UploadFileByIdAsync_WithAllHeaders_IncludesAllHeaders()
    {
        var responseJson = "{\"seqid\": 301}";
        var client = CreateClient(req =>
        {
            Assert.True(req.Headers.Contains("x-filename"));
            Assert.True(req.Headers.Contains("x-password"));
            Assert.True(req.Headers.Contains("x-language"));
            Assert.True(req.Headers.Contains("x-md5"));
            Assert.True(req.Headers.Contains("x-extract-strategy"));
            Assert.True(req.Headers.Contains("x-split-strategy"));
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
        }, out _);

        var response = await client.ResourceFields.UploadFileByIdAsync(
            "resource-123",
            "file-field",
            new byte[] { 1, 2, 3 },
            filename: "test.pdf",
            password: "secret",
            language: "en",
            md5: "d41d8cd98f00b204e9800998ecf8427e",
            extractStrategy: "paragraph",
            splitStrategy: "default"
        );

        Assert.True(response.Success);
    }

    [Fact]
    public async Task DownloadFileFieldByIdAsync_ReturnsByteArray()
    {
        var binaryContent = new byte[] { 0x25, 0x50, 0x44, 0x46 };
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(binaryContent)
        }, out var handler);

        var response = await client.ResourceFields.DownloadFileFieldByIdAsync(
            "resource-123",
            "pdf-field",
            inline: false
        );

        Assert.True(response.Success);
        Assert.Equal(binaryContent, response.Data);
        var httpReq = handler.CapturedRequests[0];
        Assert.Contains("/kb/kb123/resource/resource-123/file/pdf-field/download/field", httpReq.RequestUri!.ToString());
    }

    [Fact]
    public async Task DownloadFileFieldByIdAsync_WithInline_AddsQueryParam()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(new byte[] { 1, 2 })
        }, out var handler);

        var response = await client.ResourceFields.DownloadFileFieldByIdAsync(
            "resource-123",
            "pdf-field",
            inline: true
        );

        Assert.True(response.Success);
        var uri = handler.CapturedRequests[0].RequestUri!;
        Assert.Contains("inline=true", uri.Query);
    }

    [Fact]
    public async Task ReprocessFileFieldByIdAsync_SendsCorrectRequest()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.Accepted), out var handler);

        var response = await client.ResourceFields.ReprocessFileFieldByIdAsync(
            "resource-123",
            "pdf-field",
            resetTitle: true,
            filePassword: "secret"
        );

        Assert.True(response.Success);
        var httpReq = handler.CapturedRequests[0];
        Assert.Equal(HttpMethod.Post, httpReq.Method);
        Assert.Contains("/kb/kb123/resource/resource-123/file/pdf-field/reprocess", httpReq.RequestUri!.ToString());
        Assert.Contains("reset_title=true", httpReq.RequestUri.Query);
        Assert.True(httpReq.Headers.Contains("x-file-password"));
    }

    // ===== Link Field Tests =====

    [Fact]
    public async Task AddLinkFieldByIdAsync_SendsCorrectRequest()
    {
        var responseJson = "{\"seqid\": 400}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);

        var linkField = new LinkField("https://example.com/page");

        var response = await client.ResourceFields.AddLinkFieldByIdAsync(
            "resource-123",
            "link-field",
            linkField
        );

        Assert.True(response.Success);
        Assert.Equal(400, response.Data!.Seqid);
        var httpReq = handler.CapturedRequests[0];
        Assert.Equal(HttpMethod.Put, httpReq.Method);
        Assert.Contains("/kb/kb123/resource/resource-123/link/link-field", httpReq.RequestUri!.ToString());
    }

    [Fact]
    public async Task AddLinkFieldBySlugAsync_UsesSlugEndpoint()
    {
        var responseJson = "{\"seqid\": 401}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);

        var linkField = new LinkField("https://example.com");

        var response = await client.ResourceFields.AddLinkFieldBySlugAsync(
            "my-resource-slug",
            "link-field",
            linkField
        );

        Assert.True(response.Success);
        var httpReq = handler.CapturedRequests[0];
        Assert.Contains("/kb/kb123/slug/my-resource-slug/link/link-field", httpReq.RequestUri!.ToString());
    }

    // ===== Text Field Tests =====

    [Fact]
    public async Task AddTextFieldByIdAsync_SendsCorrectRequest()
    {
        var responseJson = "{\"seqid\": 500}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);

        var textField = new TextField("This is my text content");

        var response = await client.ResourceFields.AddTextFieldByIdAsync(
            "resource-123",
            "text-field",
            textField
        );

        Assert.True(response.Success);
        Assert.Equal(500, response.Data!.Seqid);
        var httpReq = handler.CapturedRequests[0];
        Assert.Equal(HttpMethod.Put, httpReq.Method);
        Assert.Contains("/kb/kb123/resource/resource-123/text/text-field", httpReq.RequestUri!.ToString());
    }

    [Fact]
    public async Task AddTextFieldBySlugAsync_UsesSlugEndpoint()
    {
        var responseJson = "{\"seqid\": 501}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);

        var textField = new TextField("Content");

        var response = await client.ResourceFields.AddTextFieldBySlugAsync(
            "my-resource-slug",
            "text-field",
            textField
        );

        Assert.True(response.Success);
        var httpReq = handler.CapturedRequests[0];
        Assert.Contains("/kb/kb123/slug/my-resource-slug/text/text-field", httpReq.RequestUri!.ToString());
    }

    // ===== Generic Field Operations Tests =====

    [Fact]
    public async Task GetResourceFieldByIdAsync_SendsCorrectRequest()
    {
        var responseJson = "{\"field_type\": \"text\", \"field_id\": \"my-field\"}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);

        var response = await client.ResourceFields.GetResourceFieldByIdAsync(
            "resource-123",
            FieldTypeName.Text,
            "my-field",
            show: new[] { ResourceFieldProperties.Value }
        );

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("my-field", response.Data.FieldId);
        var httpReq = handler.CapturedRequests[0];
        Assert.Contains("/kb/kb123/resource/resource-123/text/my-field", httpReq.RequestUri!.ToString());
        Assert.Contains("show=", httpReq.RequestUri.Query);
    }

    [Fact]
    public async Task GetResourceFieldBySlugAsync_UsesSlugEndpoint()
    {
        var responseJson = "{\"field_type\": \"file\", \"field_id\": \"my-file\"}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        }, out var handler);

        var response = await client.ResourceFields.GetResourceFieldBySlugAsync(
            "my-resource-slug",
            FieldTypeName.File,
            "my-file"
        );

        Assert.True(response.Success);
        var httpReq = handler.CapturedRequests[0];
        Assert.Contains("/kb/kb123/slug/my-resource-slug/file/my-file", httpReq.RequestUri!.ToString());
    }

    [Fact]
    public async Task DeleteResourceFieldByIdAsync_ReturnsSuccessOn204()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.NoContent), out var handler);

        var response = await client.ResourceFields.DeleteResourceFieldByIdAsync(
            "resource-123",
            FieldTypeName.Text,
            "old-field"
        );

        Assert.True(response.Success);
        Assert.True(response.Data);
        var httpReq = handler.CapturedRequests[0];
        Assert.Equal(HttpMethod.Delete, httpReq.Method);
        Assert.Contains("/kb/kb123/resource/resource-123/text/old-field", httpReq.RequestUri!.ToString());
    }

    [Fact]
    public async Task DeleteResourceFieldBySlugAsync_UsesSlugEndpoint()
    {
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.NoContent), out var handler);

        var response = await client.ResourceFields.DeleteResourceFieldBySlugAsync(
            "my-resource-slug",
            FieldTypeName.Conversation,
            "old-chat"
        );

        Assert.True(response.Success);
        var httpReq = handler.CapturedRequests[0];
        Assert.Contains("/kb/kb123/slug/my-resource-slug/conversation/old-chat", httpReq.RequestUri!.ToString());
    }

    [Fact]
    public async Task DownloadExtractedFileByIdAsync_ReturnsByteArray()
    {
        var binaryContent = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // JPEG magic bytes
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(binaryContent)
        }, out var handler);

        var response = await client.ResourceFields.DownloadExtractedFileByIdAsync(
            "resource-123",
            FieldTypeName.File,
            "pdf-field",
            "thumbnail"
        );

        Assert.True(response.Success);
        Assert.Equal(binaryContent, response.Data);
        var httpReq = handler.CapturedRequests[0];
        Assert.Contains("/kb/kb123/resource/resource-123/file/pdf-field/download/extracted/thumbnail", httpReq.RequestUri!.ToString());
    }

    [Fact]
    public async Task DownloadExtractedFileBySlugAsync_UsesSlugEndpoint()
    {
        var binaryContent = new byte[] { 1, 2, 3 };
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(binaryContent)
        }, out var handler);

        var response = await client.ResourceFields.DownloadExtractedFileBySlugAsync(
            "my-resource-slug",
            FieldTypeName.File,
            "doc-field",
            "preview"
        );

        Assert.True(response.Success);
        var httpReq = handler.CapturedRequests[0];
        Assert.Contains("/kb/kb123/slug/my-resource-slug/file/doc-field/download/extracted/preview", httpReq.RequestUri!.ToString());
    }

    // ===== Error Handling Tests =====

    [Fact]
    public async Task AddTextFieldByIdAsync_HandlesValidationError()
    {
        var errorJson = "{\"detail\":[{\"loc\":[\"body\",\"field\"],\"msg\":\"field required\",\"type\":\"value_error.missing\"}]}";
        var client = CreateClient(req => new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
        {
            Content = new StringContent(errorJson, Encoding.UTF8, "application/json")
        }, out _);

        var textField = new TextField("Content");

        var response = await client.ResourceFields.AddTextFieldByIdAsync(
            "resource-123",
            "text-field",
            textField
        );

        Assert.False(response.Success);
        Assert.NotNull(response.ValidationError);
    }

    [Fact]
    public async Task UploadFileByIdAsync_HandlesHttpException()
    {
        var client = CreateClient(req => throw new HttpRequestException("Network error"), out _);

        var response = await client.ResourceFields.UploadFileByIdAsync(
            "resource-123",
            "file-field",
            new byte[] { 1, 2, 3 }
        );

        Assert.False(response.Success);
        Assert.NotNull(response.Error);
        Assert.Contains("HTTP request failed", response.Error);
    }
}

using Microsoft.Extensions.Logging;
using Progress.Nuclia.Model;
using System.Net.Http.Json;

namespace Progress.Nuclia.Services;

/// <summary>
/// Implementation of Knowledge Box service operations
/// </summary>
internal class KnowledgeBoxService : BaseService, IKnowledgeBoxService
{
    public KnowledgeBoxService(HttpClient httpClient, string baseUrl, System.Text.Json.JsonSerializerOptions jsonOptions, ILogger<KnowledgeBoxService> logger)
        : base(httpClient, baseUrl, jsonOptions, logger)
    {
    }

    /// <inheritdoc />
    public async Task<ApiResponse<KnowledgeBoxObj>> GetKnowledgeBoxAsync(string knowledgeBoxId, CancellationToken cancellationToken = default)
    {
        return await ExecuteHttpRequestAsync<KnowledgeBoxObj>(
            async () => await _httpClient.GetAsync($"{_baseUrl}/kb/{knowledgeBoxId}", cancellationToken),
            nameof(GetKnowledgeBoxAsync),
            cancellationToken,
            $"with knowledgeBoxId: {knowledgeBoxId}");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<KnowledgeBoxObj>> GetKnowledgeBoxBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await ExecuteHttpRequestAsync<KnowledgeBoxObj>(
            async () => await _httpClient.GetAsync($"{_baseUrl}/kb/s/{slug}", cancellationToken),
            nameof(GetKnowledgeBoxBySlugAsync),
            cancellationToken,
            $"with slug: {slug}");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<KnowledgeboxCounters>> GetKnowledgeBoxCountersAsync(string knowledgeBoxId, bool? debug = null, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{knowledgeBoxId}/counters";
        if (debug.HasValue)
        {
            var qs = Services.HttpUtilityHelper.BuildQueryString(new Dictionary<string, string?> { ["debug"] = debug.Value.ToString().ToLowerInvariant() });
            url = Services.HttpUtilityHelper.AppendQueryString(url, qs);
        }
        return await ExecuteHttpRequestAsync<KnowledgeboxCounters>(
            async () => await _httpClient.GetAsync(url, cancellationToken),
            nameof(GetKnowledgeBoxCountersAsync),
            cancellationToken,
            $"with knowledgeBoxId: {knowledgeBoxId}, debug: {debug}");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<Dictionary<string, object>>> GetKnowledgeBoxConfigurationAsync(string knowledgeBoxId, CancellationToken cancellationToken = default)
    {
        return await ExecuteHttpRequestAsync<Dictionary<string, object>>(
            async () => await _httpClient.GetAsync($"{_baseUrl}/kb/{knowledgeBoxId}/configuration", cancellationToken),
            nameof(GetKnowledgeBoxConfigurationAsync),
            cancellationToken,
            $"with knowledgeBoxId: {knowledgeBoxId}");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<bool>> PatchKnowledgeBoxConfigurationAsync(string knowledgeBoxId, Dictionary<string, object> configurationPatch, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}/kb/{knowledgeBoxId}/configuration")
            {
                Content = new System.Net.Http.StringContent(System.Text.Json.JsonSerializer.Serialize(configurationPatch, _jsonOptions), System.Text.Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return ApiResponse<bool>.CreateSuccess(true);
            }
            response.EnsureSuccessStatusCode();
            return ApiResponse<bool>.CreateSuccess(true);
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<bool>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResponse<bool>> SetKnowledgeBoxConfigurationAsync(string knowledgeBoxId, Dictionary<string, object> configuration, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/kb/{knowledgeBoxId}/configuration")
            {
                Content = new System.Net.Http.StringContent(System.Text.Json.JsonSerializer.Serialize(configuration, _jsonOptions), System.Text.Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return ApiResponse<bool>.CreateSuccess(true);
            }
            response.EnsureSuccessStatusCode();
            return ApiResponse<bool>.CreateSuccess(true);
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<bool>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResponse<byte[]>> DownloadKnowledgeBoxExportAsync(string knowledgeBoxId, string exportId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/kb/{knowledgeBoxId}/export/{exportId}", cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                return ApiResponse<byte[]>.CreateSuccess(bytes);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                return ApiResponse<byte[]>.CreateValidationError(validationError!);
            }
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            return ApiResponse<byte[]>.CreateSuccess(data);
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<byte[]>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return ApiResponse<byte[]>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResponse<StatusResponse>> GetKnowledgeBoxExportStatusAsync(string knowledgeBoxId, string exportId, CancellationToken cancellationToken = default)
    {
        return await ExecuteHttpRequestAsync<StatusResponse>(
            async () => await _httpClient.GetAsync($"{_baseUrl}/kb/{knowledgeBoxId}/export/{exportId}/status", cancellationToken),
            nameof(GetKnowledgeBoxExportStatusAsync),
            cancellationToken,
            $"with knowledgeBoxId: {knowledgeBoxId}, exportId: {exportId}");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<StatusResponse>> GetKnowledgeBoxImportStatusAsync(string knowledgeBoxId, string importId, CancellationToken cancellationToken = default)
    {
        return await ExecuteHttpRequestAsync<StatusResponse>(
            async () => await _httpClient.GetAsync($"{_baseUrl}/kb/{knowledgeBoxId}/import/{importId}/status", cancellationToken),
            nameof(GetKnowledgeBoxImportStatusAsync),
            cancellationToken,
            $"with knowledgeBoxId: {knowledgeBoxId}, importId: {importId}");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<ResourceFileUploaded>> UploadKnowledgeBoxFileAsync(string knowledgeBoxId, Stream fileStream, string fileName, string? splitStrategy = null, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{knowledgeBoxId}/upload";

        // Read the stream into a byte array
        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        var fileContent = memoryStream.ToArray();

        // Create the content with the file bytes
        var content = new ByteArrayContent(fileContent);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        // Create the request with custom headers
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
        request.Headers.Add("x-filename", fileName);

        if (!string.IsNullOrWhiteSpace(splitStrategy))
        {
            request.Headers.Add("x-split-strategy", splitStrategy);
        }

        return await ExecuteHttpRequestAsync<ResourceFileUploaded>(
            async () => await _httpClient.SendAsync(request, cancellationToken),
            nameof(UploadKnowledgeBoxFileAsync),
            cancellationToken,
            $"with knowledgeBoxId: {knowledgeBoxId}, fileName: {fileName}, splitStrategy: {splitStrategy}");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<Dictionary<string, object>>> GetLearningConfigurationSchemaAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteHttpRequestAsync<Dictionary<string, object>>(
            async () => await _httpClient.GetAsync($"{_baseUrl}/learning/configuration/schema", cancellationToken),
            nameof(GetLearningConfigurationSchemaAsync),
            cancellationToken);
    }
}

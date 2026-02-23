using Microsoft.Extensions.Logging;
using Progress.Nuclia.Model;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Progress.Nuclia.Services;

/// <summary>
/// Implementation of Resource service operations
/// </summary>
internal class ResourceService : BaseService, IResourceService
{
    private readonly string _knowledgeBaseId;

    public ResourceService(HttpClient httpClient, string baseUrl, System.Text.Json.JsonSerializerOptions jsonOptions, string knowledgeBaseId, ILogger<ResourceService> logger)
        : base(httpClient, baseUrl, jsonOptions, logger)
    {
        _knowledgeBaseId = knowledgeBaseId;
    }

    /// <inheritdoc />
    public async Task<ApiResponse<ResourceList>> ListResourcesAsync(int? page = default, int? size = default, CancellationToken cancellationToken = default)
    {
        return await ExecuteHttpRequestAsync<ResourceList>(
            async () =>
            {
                var parameters = new Dictionary<string, string?>
                {
                    ["page"] = page?.ToString(),
                    ["size"] = size?.ToString()
                };

                var queryString = HttpUtilityHelper.BuildQueryString(parameters);
                var url = HttpUtilityHelper.AppendQueryString($"{_baseUrl}/kb/{_knowledgeBaseId}/resources", queryString);
                return await _httpClient.GetAsync(url, cancellationToken);
            },
            nameof(ListResourcesAsync),
            cancellationToken,
            $"for knowledge box {_knowledgeBaseId} with page: {page}, size: {size}");
    }

    /// <inheritdoc />
    public Task<ApiResponse<NucliadbModelsResourceResource>> GetResourceByIdAsync(
        string resourceId, 
        ResourceProperties[]? show = null,
        FieldTypeName[]? fieldType = null,
        ExtractedDataTypeName[]? extracted = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}";

        return GetResourceAsync(url, show, fieldType, extracted, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<NucliadbModelsResourceResource>> GetResourceBySlugAsync(
        string resourceSlug,
        ResourceProperties[]? show = null,
        FieldTypeName[]? fieldType = null,
        ExtractedDataTypeName[]? extracted = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}";

        return GetResourceAsync(url, show, fieldType, extracted, cancellationToken);
    }

    private async Task<ApiResponse<NucliadbModelsResourceResource>> GetResourceAsync(
        string url,
        ResourceProperties[]? show = null,
        FieldTypeName[]? fieldType = null,
        ExtractedDataTypeName[]? extracted = null,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteHttpRequestAsync<NucliadbModelsResourceResource>(
            async () =>
            {
                var parameters = new Dictionary<string, object?>();

                if (show != null && show.Length > 0)
                    parameters["show"] = show;
                if (fieldType != null && fieldType.Length > 0)
                    parameters["field_type"] = fieldType;
                if (extracted != null && extracted.Length > 0)
                    parameters["extracted"] = extracted;

                var queryString = HttpUtilityHelper.BuildQueryStringWithArrays(parameters);
                var fullUrl = HttpUtilityHelper.AppendQueryString(url, queryString);
                return await _httpClient.GetAsync(fullUrl, cancellationToken);
            },
            nameof(GetResourceAsync),
            cancellationToken,
            $"with url: {url}");
    }

    /// <inheritdoc />
    public Task<ApiResponse<bool>> DeleteResourceByIdAsync(string resourceId, CancellationToken cancellationToken = default)
    {
        return DeleteResourceAsync($"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}", cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<bool>> DeleteResourceBySlugAsync(string resourceSlug, CancellationToken cancellationToken = default)
    {
        return DeleteResourceAsync($"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}", cancellationToken);
    }

    private async Task<ApiResponse<bool>> DeleteResourceAsync(string url, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting {OperationName} with url: {Url}", nameof(DeleteResourceAsync), url);
        try
        {
            var response = await _httpClient.DeleteAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(DeleteResourceAsync), validationError);
                return ApiResponse<bool>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            _logger.LogInformation("{OperationName} completed successfully", nameof(DeleteResourceAsync));
            return ApiResponse<bool>.CreateSuccess(true);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(DeleteResourceAsync));
            return ApiResponse<bool>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(DeleteResourceAsync));
            return ApiResponse<bool>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResponse<byte[]>> DownloadFieldFileAsync(string resourceId, string fieldId, bool inline = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting {OperationName} with resourceId: {ResourceId}, fieldId: {FieldId}, inline: {Inline}", 
            nameof(DownloadFieldFileAsync), resourceId, fieldId, inline);
        try
        {
            var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/file/{fieldId}/download/field";

            if (inline)
            {
                var queryString = HttpUtilityHelper.BuildQueryString(new Dictionary<string, string?>
                {
                    ["inline"] = inline.ToString().ToLowerInvariant()
                });
                url = HttpUtilityHelper.AppendQueryString(url, queryString);
            }

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(DownloadFieldFileAsync), validationError);
                return ApiResponse<byte[]>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            _logger.LogInformation("{OperationName} completed successfully, downloaded {Size} bytes", nameof(DownloadFieldFileAsync), content.Length);
            return ApiResponse<byte[]>.CreateSuccess(content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(DownloadFieldFileAsync));
            return ApiResponse<byte[]>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(DownloadFieldFileAsync));
            return ApiResponse<byte[]>.CreateError($"Unexpected error: {ex.Message}");
        }
    }
    /// <inheritdoc />
    public async Task<ApiResponse<byte[]>> DownloadFieldFileAsync(string resourceUrl, bool inline = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting {OperationName} with resourceUrl: {ResourceUrl}, inline: {Inline}", 
            nameof(DownloadFieldFileAsync), resourceUrl, inline);
        try
        {
            var url = $"{_baseUrl}{resourceUrl}";

            if (inline)
            {
                var queryString = HttpUtilityHelper.BuildQueryString(new Dictionary<string, string?>
                {
                    ["inline"] = inline.ToString().ToLowerInvariant()
                });
                url = HttpUtilityHelper.AppendQueryString(url, queryString);
            }

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(DownloadFieldFileAsync), validationError);
                return ApiResponse<byte[]>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            _logger.LogInformation("{OperationName} completed successfully, downloaded {Size} bytes", nameof(DownloadFieldFileAsync), content.Length);
            return ApiResponse<byte[]>.CreateSuccess(content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(DownloadFieldFileAsync));
            return ApiResponse<byte[]>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(DownloadFieldFileAsync));
            return ApiResponse<byte[]>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResponse<ResourceCreated>> CreateResourceAsync(CreateResourcePayload request, bool skipStore = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting {OperationName} for knowledge box {KnowledgeBoxId}, skipStore: {SkipStore}", 
            nameof(CreateResourceAsync), _knowledgeBaseId, skipStore);
        try
        {
            var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resources";

            // Create HTTP request message to add custom headers
            using var httpRequest = HttpRequestHelper.CreateJsonRequest(HttpMethod.Post, url, request, _jsonOptions);

            if (skipStore)
            {
                httpRequest.AddBooleanHeader("x-skip-store", skipStore);
            }

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(CreateResourceAsync), validationError);
                return ApiResponse<ResourceCreated>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            var createResponse = await response.Content.ReadFromJsonAsync<ResourceCreated>(_jsonOptions, cancellationToken);
            _logger.LogInformation("{OperationName} completed successfully", nameof(CreateResourceAsync));
            return ApiResponse<ResourceCreated>.CreateSuccess(createResponse!);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(CreateResourceAsync));
            return ApiResponse<ResourceCreated>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(CreateResourceAsync));
            return ApiResponse<ResourceCreated>.CreateError($"Unexpected error: {ex.Message}");
        }
    }
    /// <inheritdoc />
    public Task<ApiResponse<bool>> ReindexResourceByIdAsync(string resourceId, bool? reindexVectors = null, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/reindex";
        return ReindexResourceAsync(url, reindexVectors, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<bool>> ReindexResourceBySlugAsync(string resourceSlug, bool? reindexVectors = null, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}/reindex";
        return ReindexResourceAsync(url, reindexVectors, cancellationToken);
    }

    private async Task<ApiResponse<bool>> ReindexResourceAsync(string url, bool? reindexVectors, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting {OperationName} with url: {Url}, reindexVectors: {ReindexVectors}", nameof(ReindexResourceAsync), url, reindexVectors);
        try
        {
            if (reindexVectors.HasValue)
            {
                var qs = HttpUtilityHelper.BuildQueryString(new Dictionary<string, string?>
                {
                    ["reindex_vectors"] = reindexVectors.Value.ToString().ToLowerInvariant()
                });
                url = HttpUtilityHelper.AppendQueryString(url, qs);
            }

            var response = await _httpClient.PostAsync(url, content: null, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(ReindexResourceAsync), validationError);
                return ApiResponse<bool>.CreateValidationError(validationError!);
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                _logger.LogInformation("{OperationName} completed (204 No Content)", nameof(ReindexResourceAsync));
                return ApiResponse<bool>.CreateSuccess(true);
            }

            response.EnsureSuccessStatusCode();
            _logger.LogInformation("{OperationName} completed with success code {StatusCode}", nameof(ReindexResourceAsync), response.StatusCode);
            return ApiResponse<bool>.CreateSuccess(true);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(ReindexResourceAsync));
            return ApiResponse<bool>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(ReindexResourceAsync));
            return ApiResponse<bool>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public Task<ApiResponse<ResourceUpdated>> ReprocessResourceByIdAsync(string resourceId, bool? resetTitle = null, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/reprocess";
        return ReprocessResourceAsync(url, resetTitle, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<ResourceUpdated>> ReprocessResourceBySlugAsync(string resourceSlug, bool? resetTitle = null, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}/reprocess";
        return ReprocessResourceAsync(url, resetTitle, cancellationToken);
    }

    private Task<ApiResponse<ResourceUpdated>> ReprocessResourceAsync(string url, bool? resetTitle, CancellationToken cancellationToken)
    {
        if (resetTitle.HasValue)
        {
            var qs = HttpUtilityHelper.BuildQueryString(new Dictionary<string, string?>
            {
                ["reset_title"] = resetTitle.Value.ToString().ToLowerInvariant()
            });
            url = HttpUtilityHelper.AppendQueryString(url, qs);
        }

        return ExecuteHttpRequestAsync<ResourceUpdated>(
            async () => await _httpClient.PostAsync(url, content: null, cancellationToken),
            nameof(ReprocessResourceAsync),
            cancellationToken,
            $"url: {url}, resetTitle: {resetTitle}");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<ResourceUpdated>> ModifyResourceBySlugAsync(string resourceSlug, UpdateResourcePayload payload, bool skipStore = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting ModifyResourceBySlugAsync for resource slug {ResourceSlug} in knowledge box {KnowledgeBoxId}", resourceSlug, _knowledgeBaseId);
        
        return await ExecuteHttpRequestAsync<ResourceUpdated>(
            async () =>
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}");
                httpRequest.Content = JsonContent.Create(payload, options: _jsonOptions);
                if (skipStore)
                {
                    httpRequest.Headers.Add("x-skip-store", "true");
                }
                return await _httpClient.SendAsync(httpRequest, cancellationToken);
            },
            nameof(ModifyResourceBySlugAsync),
            cancellationToken,
            $"for resource slug {resourceSlug} in knowledge box {_knowledgeBaseId}");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<ResourceUpdated>> ModifyResourceByIdAsync(string resourceId, UpdateResourcePayload payload, bool skipStore = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting ModifyResourceByIdAsync for resource ID {ResourceId} in knowledge box {KnowledgeBoxId}", resourceId, _knowledgeBaseId);
        
        return await ExecuteHttpRequestAsync<ResourceUpdated>(
            async () =>
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}");
                httpRequest.Content = JsonContent.Create(payload, options: _jsonOptions);
                if (skipStore)
                {
                    httpRequest.Headers.Add("x-skip-store", "true");
                }
                return await _httpClient.SendAsync(httpRequest, cancellationToken);
            },
            nameof(ModifyResourceByIdAsync),
            cancellationToken,
            $"for resource ID {resourceId} in knowledge box {_knowledgeBaseId}");
    }

    /// <inheritdoc />
    public Task<ApiResponse<ResourceAgentsResponse>> RunAgentsOnResourceByIdAsync(string resourceId, ResourceAgentsRequest request, CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/run-agents";
        return ExecuteHttpRequestAsync<ResourceAgentsResponse>(
            async () =>
            {
                using var httpRequest = HttpRequestHelper.CreateJsonRequest(HttpMethod.Post, url, request, _jsonOptions);
                return await _httpClient.SendAsync(httpRequest, cancellationToken);
            },
            nameof(RunAgentsOnResourceByIdAsync),
            cancellationToken,
            $"resourceId: {resourceId}");
    }

    /// <inheritdoc />
    public Task<ApiResponse<ResourceSearchResults>> SearchWithinResourceByIdAsync(
        string resourceId,
        string query,
        string[]? fields = null,
        string[]? filters = null,
        string[]? faceted = null,
        string? filterExpression = null,
        SortField? sortField = null,
        SortOrder? sortOrder = null,
        int? topK = null,
        DateTime? rangeCreationStart = null,
        DateTime? rangeCreationEnd = null,
        DateTime? rangeModificationStart = null,
        DateTime? rangeModificationEnd = null,
        bool? highlight = null,
        bool? debug = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/search";
        return SearchWithinResourceAsync(url, query, fields, filters, faceted, filterExpression, sortField, sortOrder, topK,
            rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, highlight, debug, cancellationToken);
    }

    private Task<ApiResponse<ResourceSearchResults>> SearchWithinResourceAsync(
        string url,
        string query,
        string[]? fields,
        string[]? filters,
        string[]? faceted,
        string? filterExpression,
        SortField? sortField,
        SortOrder? sortOrder,
        int? topK,
        DateTime? rangeCreationStart,
        DateTime? rangeCreationEnd,
        DateTime? rangeModificationStart,
        DateTime? rangeModificationEnd,
        bool? highlight,
        bool? debug,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Task.FromResult(ApiResponse<ResourceSearchResults>.CreateError("Query parameter is required."));
        }

        return ExecuteHttpRequestAsync<ResourceSearchResults>(
            async () =>
            {
                var parameters = new Dictionary<string, object?>
                {
                    ["query"] = query,
                    ["filter_expression"] = filterExpression,
                    ["sort_field"] = sortField,
                    ["sort_order"] = sortOrder,
                    ["top_k"] = topK,
                    ["range_creation_start"] = rangeCreationStart?.ToString("o"),
                    ["range_creation_end"] = rangeCreationEnd?.ToString("o"),
                    ["range_modification_start"] = rangeModificationStart?.ToString("o"),
                    ["range_modification_end"] = rangeModificationEnd?.ToString("o"),
                    ["highlight"] = highlight,
                    ["debug"] = debug
                };

                if (fields != null && fields.Length > 0) parameters["fields"] = fields;
                if (filters != null && filters.Length > 0) parameters["filters"] = filters;
                if (faceted != null && faceted.Length > 0) parameters["faceted"] = faceted;

                var queryString = HttpUtilityHelper.BuildQueryStringWithArrays(parameters);
                var fullUrl = HttpUtilityHelper.AppendQueryString(url, queryString);
                return await _httpClient.GetAsync(fullUrl, cancellationToken);
            },
            nameof(SearchWithinResourceAsync),
            cancellationToken,
            $"url: {url}, query: {query}");
    }
}

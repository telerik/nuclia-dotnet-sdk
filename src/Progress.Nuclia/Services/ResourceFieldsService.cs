using Microsoft.Extensions.Logging;
using Progress.Nuclia.Model;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Progress.Nuclia.Services;

/// <summary>
/// Implementation of Resource Fields service operations
/// </summary>
internal class ResourceFieldsService : BaseService, IResourceFieldsService
{
    private readonly string _knowledgeBaseId;

    public ResourceFieldsService(HttpClient httpClient, string baseUrl, System.Text.Json.JsonSerializerOptions jsonOptions, string knowledgeBaseId, ILogger<ResourceFieldsService> logger)
        : base(httpClient, baseUrl, jsonOptions, logger)
    {
        _knowledgeBaseId = knowledgeBaseId;
    }

    #region Conversation Field Operations

    /// <inheritdoc />
    public Task<ApiResponse<ResourceFieldAdded>> AddConversationFieldByIdAsync(
        string resourceId,
        string fieldId,
        InputConversationField conversationField,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/conversation/{fieldId}";
        return AddConversationFieldAsync(url, conversationField, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<ResourceFieldAdded>> AddConversationFieldBySlugAsync(
        string resourceSlug,
        string fieldId,
        InputConversationField conversationField,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}/conversation/{fieldId}";
        return AddConversationFieldAsync(url, conversationField, cancellationToken);
    }

    private async Task<ApiResponse<ResourceFieldAdded>> AddConversationFieldAsync(
        string url,
        InputConversationField conversationField,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting {OperationName} with url: {Url}", nameof(AddConversationFieldAsync), url);
        try
        {
            var content = JsonContent.Create(conversationField, options: _jsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Put, url) { Content = content };
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(AddConversationFieldAsync), validationError);
                return ApiResponse<ResourceFieldAdded>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ResourceFieldAdded>(_jsonOptions, cancellationToken);
            _logger.LogInformation("{OperationName} completed successfully", nameof(AddConversationFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateSuccess(result!);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(AddConversationFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(AddConversationFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public Task<ApiResponse<ResourceFieldAdded>> AppendMessagesToConversationFieldByIdAsync(
        string resourceId,
        string fieldId,
        List<InputMessage> messages,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/conversation/{fieldId}/messages";
        return AppendMessagesToConversationFieldAsync(url, messages, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<ResourceFieldAdded>> AppendMessagesToConversationFieldBySlugAsync(
        string resourceSlug,
        string fieldId,
        List<InputMessage> messages,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}/conversation/{fieldId}/messages";
        return AppendMessagesToConversationFieldAsync(url, messages, cancellationToken);
    }

    private async Task<ApiResponse<ResourceFieldAdded>> AppendMessagesToConversationFieldAsync(
        string url,
        List<InputMessage> messages,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting {OperationName} with url: {Url}, messageCount: {MessageCount}", 
            nameof(AppendMessagesToConversationFieldAsync), url, messages.Count);
        try
        {
            var content = JsonContent.Create(messages, options: _jsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Put, url) { Content = content };
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(AppendMessagesToConversationFieldAsync), validationError);
                return ApiResponse<ResourceFieldAdded>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ResourceFieldAdded>(_jsonOptions, cancellationToken);
            _logger.LogInformation("{OperationName} completed successfully", nameof(AppendMessagesToConversationFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateSuccess(result!);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(AppendMessagesToConversationFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(AppendMessagesToConversationFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public Task<ApiResponse<byte[]>> DownloadConversationAttachmentByIdAsync(
        string resourceId,
        string fieldId,
        string messageId,
        int fileNum,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/conversation/{fieldId}/download/field/{messageId}/{fileNum}";
        return DownloadBinaryAsync(url, nameof(DownloadConversationAttachmentByIdAsync), cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<byte[]>> DownloadConversationAttachmentBySlugAsync(
        string resourceSlug,
        string fieldId,
        string messageId,
        int fileNum,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}/conversation/{fieldId}/download/field/{messageId}/{fileNum}";
        return DownloadBinaryAsync(url, nameof(DownloadConversationAttachmentBySlugAsync), cancellationToken);
    }

    #endregion

    #region File Field Operations

    /// <inheritdoc />
    public Task<ApiResponse<ResourceFieldAdded>> AddFileFieldByIdAsync(
        string resourceId,
        string fieldId,
        FileField fileField,
        bool skipStore = false,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/file/{fieldId}";
        return AddFileFieldAsync(url, fileField, skipStore, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<ResourceFieldAdded>> AddFileFieldBySlugAsync(
        string resourceSlug,
        string fieldId,
        FileField fileField,
        bool skipStore = false,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}/file/{fieldId}";
        return AddFileFieldAsync(url, fileField, skipStore, cancellationToken);
    }

    private async Task<ApiResponse<ResourceFieldAdded>> AddFileFieldAsync(
        string url,
        FileField fileField,
        bool skipStore,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting {OperationName} with url: {Url}, skipStore: {SkipStore}", 
            nameof(AddFileFieldAsync), url, skipStore);
        try
        {
            var content = JsonContent.Create(fileField, options: _jsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Put, url) { Content = content };
            
            if (skipStore)
            {
                request.Headers.Add("x-skip-store", "true");
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(AddFileFieldAsync), validationError);
                return ApiResponse<ResourceFieldAdded>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ResourceFieldAdded>(_jsonOptions, cancellationToken);
            _logger.LogInformation("{OperationName} completed successfully", nameof(AddFileFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateSuccess(result!);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(AddFileFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(AddFileFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public Task<ApiResponse<ResourceFileUploaded>> UploadFileByIdAsync(
        string resourceId,
        string fieldId,
        byte[] fileContent,
        string? filename = null,
        string? password = null,
        string? language = null,
        string? md5 = null,
        string? extractStrategy = null,
        string? splitStrategy = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/file/{fieldId}/upload";
        return UploadFileAsync(url, fileContent, filename, password, language, md5, extractStrategy, splitStrategy, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<ResourceFileUploaded>> UploadFileBySlugAsync(
        string resourceSlug,
        string fieldId,
        byte[] fileContent,
        string? filename = null,
        string? password = null,
        string? language = null,
        string? md5 = null,
        string? extractStrategy = null,
        string? splitStrategy = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}/file/{fieldId}/upload";
        return UploadFileAsync(url, fileContent, filename, password, language, md5, extractStrategy, splitStrategy, cancellationToken);
    }

    private async Task<ApiResponse<ResourceFileUploaded>> UploadFileAsync(
        string url,
        byte[] fileContent,
        string? filename,
        string? password,
        string? language,
        string? md5,
        string? extractStrategy,
        string? splitStrategy,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting {OperationName} with url: {Url}, fileSize: {FileSize}, filename: {Filename}", 
            nameof(UploadFileAsync), url, fileContent.Length, filename);
        try
        {
            var content = new ByteArrayContent(fileContent);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            
            if (!string.IsNullOrEmpty(filename))
                request.Headers.Add("x-filename", filename);
            if (!string.IsNullOrEmpty(password))
                request.Headers.Add("x-password", password);
            if (!string.IsNullOrEmpty(language))
                request.Headers.Add("x-language", language);
            if (!string.IsNullOrEmpty(md5))
                request.Headers.Add("x-md5", md5);
            if (!string.IsNullOrEmpty(extractStrategy))
                request.Headers.Add("x-extract-strategy", extractStrategy);
            if (!string.IsNullOrEmpty(splitStrategy))
                request.Headers.Add("x-split-strategy", splitStrategy);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(UploadFileAsync), validationError);
                return ApiResponse<ResourceFileUploaded>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ResourceFileUploaded>(_jsonOptions, cancellationToken);
            _logger.LogInformation("{OperationName} completed successfully", nameof(UploadFileAsync));
            return ApiResponse<ResourceFileUploaded>.CreateSuccess(result!);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(UploadFileAsync));
            return ApiResponse<ResourceFileUploaded>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(UploadFileAsync));
            return ApiResponse<ResourceFileUploaded>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public Task<ApiResponse<byte[]>> DownloadFileFieldByIdAsync(
        string resourceId,
        string fieldId,
        bool inline = false,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/file/{fieldId}/download/field";
        return DownloadFileFieldAsync(url, inline, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<byte[]>> DownloadFileFieldBySlugAsync(
        string resourceSlug,
        string fieldId,
        bool inline = false,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}/file/{fieldId}/download/field";
        return DownloadFileFieldAsync(url, inline, cancellationToken);
    }

    private async Task<ApiResponse<byte[]>> DownloadFileFieldAsync(
        string url,
        bool inline,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting {OperationName} with url: {Url}, inline: {Inline}", 
            nameof(DownloadFileFieldAsync), url, inline);
        
        if (inline)
        {
            var queryString = HttpUtilityHelper.BuildQueryString(new Dictionary<string, string?>
            {
                ["inline"] = inline.ToString().ToLowerInvariant()
            });
            url = HttpUtilityHelper.AppendQueryString(url, queryString);
        }

        return await DownloadBinaryAsync(url, nameof(DownloadFileFieldAsync), cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<bool>> ReprocessFileFieldByIdAsync(
        string resourceId,
        string fieldId,
        bool? resetTitle = null,
        string? filePassword = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/file/{fieldId}/reprocess";
        return ReprocessFileFieldAsync(url, resetTitle, filePassword, cancellationToken);
    }

    private async Task<ApiResponse<bool>> ReprocessFileFieldAsync(
        string url,
        bool? resetTitle,
        string? filePassword,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting {OperationName} with url: {Url}, resetTitle: {ResetTitle}", 
            nameof(ReprocessFileFieldAsync), url, resetTitle);
        try
        {
            if (resetTitle.HasValue)
            {
                var qs = HttpUtilityHelper.BuildQueryString(new Dictionary<string, string?>
                {
                    ["reset_title"] = resetTitle.Value.ToString().ToLowerInvariant()
                });
                url = HttpUtilityHelper.AppendQueryString(url, qs);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            
            if (!string.IsNullOrEmpty(filePassword))
            {
                request.Headers.Add("x-file-password", filePassword);
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(ReprocessFileFieldAsync), validationError);
                return ApiResponse<bool>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            _logger.LogInformation("{OperationName} completed successfully", nameof(ReprocessFileFieldAsync));
            return ApiResponse<bool>.CreateSuccess(true);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(ReprocessFileFieldAsync));
            return ApiResponse<bool>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(ReprocessFileFieldAsync));
            return ApiResponse<bool>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    #endregion

    #region Link Field Operations

    /// <inheritdoc />
    public Task<ApiResponse<ResourceFieldAdded>> AddLinkFieldByIdAsync(
        string resourceId,
        string fieldId,
        LinkField linkField,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/link/{fieldId}";
        return AddLinkFieldAsync(url, linkField, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<ResourceFieldAdded>> AddLinkFieldBySlugAsync(
        string resourceSlug,
        string fieldId,
        LinkField linkField,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}/link/{fieldId}";
        return AddLinkFieldAsync(url, linkField, cancellationToken);
    }

    private async Task<ApiResponse<ResourceFieldAdded>> AddLinkFieldAsync(
        string url,
        LinkField linkField,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting {OperationName} with url: {Url}", nameof(AddLinkFieldAsync), url);
        try
        {
            var content = JsonContent.Create(linkField, options: _jsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Put, url) { Content = content };
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(AddLinkFieldAsync), validationError);
                return ApiResponse<ResourceFieldAdded>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ResourceFieldAdded>(_jsonOptions, cancellationToken);
            _logger.LogInformation("{OperationName} completed successfully", nameof(AddLinkFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateSuccess(result!);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(AddLinkFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(AddLinkFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    #endregion

    #region Text Field Operations

    /// <inheritdoc />
    public Task<ApiResponse<ResourceFieldAdded>> AddTextFieldByIdAsync(
        string resourceId,
        string fieldId,
        TextField textField,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/text/{fieldId}";
        return AddTextFieldAsync(url, textField, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<ResourceFieldAdded>> AddTextFieldBySlugAsync(
        string resourceSlug,
        string fieldId,
        TextField textField,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}/text/{fieldId}";
        return AddTextFieldAsync(url, textField, cancellationToken);
    }

    private async Task<ApiResponse<ResourceFieldAdded>> AddTextFieldAsync(
        string url,
        TextField textField,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting {OperationName} with url: {Url}", nameof(AddTextFieldAsync), url);
        try
        {
            var content = JsonContent.Create(textField, options: _jsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Put, url) { Content = content };
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(AddTextFieldAsync), validationError);
                return ApiResponse<ResourceFieldAdded>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ResourceFieldAdded>(_jsonOptions, cancellationToken);
            _logger.LogInformation("{OperationName} completed successfully", nameof(AddTextFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateSuccess(result!);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(AddTextFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(AddTextFieldAsync));
            return ApiResponse<ResourceFieldAdded>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    #endregion

    #region Generic Field Operations

    /// <inheritdoc />
    public Task<ApiResponse<ResourceField>> GetResourceFieldByIdAsync(
        string resourceId,
        FieldTypeName fieldType,
        string fieldId,
        ResourceFieldProperties[]? show = null,
        ExtractedDataTypeName[]? extracted = null,
        string? page = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/{GetFieldTypeString(fieldType)}/{fieldId}";
        return GetResourceFieldAsync(url, show, extracted, page, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<ResourceField>> GetResourceFieldBySlugAsync(
        string resourceSlug,
        FieldTypeName fieldType,
        string fieldId,
        ResourceFieldProperties[]? show = null,
        ExtractedDataTypeName[]? extracted = null,
        string? page = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}/{GetFieldTypeString(fieldType)}/{fieldId}";
        return GetResourceFieldAsync(url, show, extracted, page, cancellationToken);
    }

    private async Task<ApiResponse<ResourceField>> GetResourceFieldAsync(
        string url,
        ResourceFieldProperties[]? show,
        ExtractedDataTypeName[]? extracted,
        string? page,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting {OperationName} with url: {Url}", nameof(GetResourceFieldAsync), url);
        try
        {
            var parameters = new Dictionary<string, object?>();

            if (show != null && show.Length > 0)
                parameters["show"] = show;
            if (extracted != null && extracted.Length > 0)
                parameters["extracted"] = extracted;
            if (!string.IsNullOrEmpty(page))
                parameters["page"] = page;

            var queryString = HttpUtilityHelper.BuildQueryStringWithArrays(parameters);
            var fullUrl = HttpUtilityHelper.AppendQueryString(url, queryString);
            
            var response = await _httpClient.GetAsync(fullUrl, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(GetResourceFieldAsync), validationError);
                return ApiResponse<ResourceField>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ResourceField>(_jsonOptions, cancellationToken);
            _logger.LogInformation("{OperationName} completed successfully", nameof(GetResourceFieldAsync));
            return ApiResponse<ResourceField>.CreateSuccess(result!);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(GetResourceFieldAsync));
            return ApiResponse<ResourceField>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(GetResourceFieldAsync));
            return ApiResponse<ResourceField>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public Task<ApiResponse<bool>> DeleteResourceFieldByIdAsync(
        string resourceId,
        FieldTypeName fieldType,
        string fieldId,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/{GetFieldTypeString(fieldType)}/{fieldId}";
        return DeleteResourceFieldAsync(url, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<bool>> DeleteResourceFieldBySlugAsync(
        string resourceSlug,
        FieldTypeName fieldType,
        string fieldId,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}/{GetFieldTypeString(fieldType)}/{fieldId}";
        return DeleteResourceFieldAsync(url, cancellationToken);
    }

    private async Task<ApiResponse<bool>> DeleteResourceFieldAsync(
        string url,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting {OperationName} with url: {Url}", nameof(DeleteResourceFieldAsync), url);
        try
        {
            var response = await _httpClient.DeleteAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", nameof(DeleteResourceFieldAsync), validationError);
                return ApiResponse<bool>.CreateValidationError(validationError!);
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                _logger.LogInformation("{OperationName} completed (204 No Content)", nameof(DeleteResourceFieldAsync));
                return ApiResponse<bool>.CreateSuccess(true);
            }

            response.EnsureSuccessStatusCode();
            _logger.LogInformation("{OperationName} completed successfully", nameof(DeleteResourceFieldAsync));
            return ApiResponse<bool>.CreateSuccess(true);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", nameof(DeleteResourceFieldAsync));
            return ApiResponse<bool>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", nameof(DeleteResourceFieldAsync));
            return ApiResponse<bool>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public Task<ApiResponse<byte[]>> DownloadExtractedFileByIdAsync(
        string resourceId,
        FieldTypeName fieldType,
        string fieldId,
        string downloadField,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/resource/{resourceId}/{GetFieldTypeString(fieldType)}/{fieldId}/download/extracted/{downloadField}";
        return DownloadBinaryAsync(url, nameof(DownloadExtractedFileByIdAsync), cancellationToken);
    }

    /// <inheritdoc />
    public Task<ApiResponse<byte[]>> DownloadExtractedFileBySlugAsync(
        string resourceSlug,
        FieldTypeName fieldType,
        string fieldId,
        string downloadField,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/kb/{_knowledgeBaseId}/slug/{resourceSlug}/{GetFieldTypeString(fieldType)}/{fieldId}/download/extracted/{downloadField}";
        return DownloadBinaryAsync(url, nameof(DownloadExtractedFileBySlugAsync), cancellationToken);
    }

    #endregion

    #region Helper Methods

    private async Task<ApiResponse<byte[]>> DownloadBinaryAsync(
        string url,
        string operationName,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting {OperationName} with url: {Url}", operationName, url);
        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", operationName, validationError);
                return ApiResponse<byte[]>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            _logger.LogInformation("{OperationName} completed successfully, downloaded {Size} bytes", operationName, content.Length);
            return ApiResponse<byte[]>.CreateSuccess(content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", operationName);
            return ApiResponse<byte[]>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", operationName);
            return ApiResponse<byte[]>.CreateError($"Unexpected error: {ex.Message}");
        }
    }

    private string GetFieldTypeString(FieldTypeName fieldType)
    {
        return fieldType switch
        {
            FieldTypeName.Text => "text",
            FieldTypeName.File => "file",
            FieldTypeName.Link => "link",
            FieldTypeName.Conversation => "conversation",
            FieldTypeName.Generic => "generic",
            _ => fieldType.ToString().ToLowerInvariant()
        };
    }

    #endregion
}

using Microsoft.Extensions.Logging;
using Progress.Nuclia.Model;
using Progress.Nuclia.Model.Streaming;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Progress.Nuclia.Services;

/// <summary>
/// Implementation of Search service operations
/// </summary>
internal class SearchService : BaseService, ISearchService
{
	private readonly string _knowledgeBoxId;

	public SearchService(HttpClient httpClient, string baseUrl, System.Text.Json.JsonSerializerOptions jsonOptions, string knowledgeBaseId, ILogger<SearchService> logger)
		: base(httpClient, baseUrl, jsonOptions, logger)
	{
		_knowledgeBoxId = knowledgeBaseId;
	}

	/// <inheritdoc />
	public async Task<ApiResponse<SyncAskResponse>> AskAsync(AskRequest request, CancellationToken cancellationToken = default)
	{
		return await ExecuteHttpRequestAsync<SyncAskResponse>(
			async () =>
			{
				using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/kb/{_knowledgeBoxId}/ask");
				httpRequest.AddBooleanHeader("x-synchronous", true);
				httpRequest.Content = JsonContent.Create(request, options: _jsonOptions);
				return await _httpClient.SendAsync(httpRequest, cancellationToken);
			},
			nameof(AskAsync),
			cancellationToken,
			$"for knowledge box {_knowledgeBoxId}");
	}

	public async Task<ApiResponse<T>> AskAsync<T>(AskRequest request, CancellationToken cancellationToken = default)
	{
		_logger.LogDebug("Starting AskAsync<{TypeName}> for knowledge box {KnowledgeBoxId}", typeof(T).Name, _knowledgeBoxId);

		// Generate JSON schema from type T using the built-in JsonSchemaExporter
		var jsonSchema = JsonSchemaGenerator.GenerateSchema<T>(_jsonOptions);

		// Assign schema to request
		request.AnswerJsonSchema = jsonSchema;

		// Call the existing AskAsync method
		var response = await AskAsync(request, cancellationToken);

		// Check if the request was successful
		if (!response.Success || response.Data == null)
		{
			_logger.LogWarning("AskAsync<{TypeName}> failed for knowledge box {KnowledgeBoxId}: {Error}",
				typeof(T).Name, _knowledgeBoxId, response.Error);
			return ApiResponse<T>.CreateError(response.Error ?? "Request failed");
		}

		// Check if AnswerJson is present
		if (response.Data.AnswerJson == null)
		{
			_logger.LogWarning("AskAsync<{TypeName}> received null AnswerJson for knowledge box {KnowledgeBoxId}",
				typeof(T).Name, _knowledgeBoxId);
			return ApiResponse<T>.CreateError("Response did not contain structured JSON answer");
		}

		try
		{
			// Serialize the AnswerJson dictionary back to JSON string
			var jsonString = JsonSerializer.Serialize(response.Data.AnswerJson, _jsonOptions);

			// Deserialize to type T
			var typedResult = JsonSerializer.Deserialize<T>(jsonString, _jsonOptions);

			if (typedResult == null)
			{
				_logger.LogWarning("AskAsync<{TypeName}> failed to deserialize AnswerJson to type {TypeName} for knowledge box {KnowledgeBoxId}",
					typeof(T).Name, typeof(T).Name, _knowledgeBoxId);
				return ApiResponse<T>.CreateError($"Failed to deserialize response to type {typeof(T).Name}");
			}

			_logger.LogInformation("AskAsync<{TypeName}> completed successfully for knowledge box {KnowledgeBoxId}",
				typeof(T).Name, _knowledgeBoxId);

			return ApiResponse<T>.CreateSuccess(typedResult);
		}
		catch (JsonException ex)
		{
			_logger.LogError(ex, "AskAsync<{TypeName}> JSON deserialization failed for knowledge box {KnowledgeBoxId}",
				typeof(T).Name, _knowledgeBoxId);
			return ApiResponse<T>.CreateError($"Failed to deserialize response: {ex.Message}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "AskAsync<{TypeName}> unexpected error for knowledge box {KnowledgeBoxId}",
				typeof(T).Name, _knowledgeBoxId);
			return ApiResponse<T>.CreateError($"Unexpected error: {ex.Message}");
		}
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<SyncAskResponseUpdate> AskStreamAsync(AskRequest request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		_logger.LogDebug("Starting AskStreamAsync for knowledge box {KnowledgeBoxId}", _knowledgeBoxId);
		if (request.AnswerJsonSchema != null)
		{
			throw new NotSupportedException("Structured JSON responses (AnswerJsonSchema) cannot be used with streaming requests because the schema requires a complete response. Use the non-streaming AskAsync<T>() method instead to get typed results.");
		}
		HttpResponseMessage response;
		try
		{
			using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/kb/{_knowledgeBoxId}/ask");
			httpRequest.Content = JsonContent.Create(request, options: _jsonOptions);
			response = await _httpClient.SendAsync(httpRequest, cancellationToken);
			response.EnsureSuccessStatusCode();
		}
		catch (HttpRequestException ex)
		{
			_logger.LogError(ex, "AskStreamAsync HTTP request failed");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "AskStreamAsync unexpected error during request");
			throw;
		}

        string? learningId = null;
        // Get value for the nuclia-learning-id Header if present
        if (response.Headers.TryGetValues("nuclia-learning-id", out var learningIdValues))
        {
            learningId = learningIdValues.FirstOrDefault();
        }

		await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
		using var reader = new StreamReader(stream);

		while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
		{
			var line = await reader.ReadLineAsync();
			if (string.IsNullOrEmpty(line)) continue;

			var streamResponse = JsonSerializer.Deserialize<SyncAskResponseUpdate>(line, _jsonOptions);
			if (streamResponse != null)
			{
                streamResponse.LearningId = learningId;
                
				yield return streamResponse;
			}
		}

		_logger.LogInformation("AskStreamAsync completed successfully");
		response.Dispose();
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<SyncAskResponseUpdate> AskStreamAsync(string query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		_logger.LogDebug("AskStreamAsync called with query: {Query}", query);
		await foreach (var response in AskStreamAsync(new AskRequest(query), cancellationToken))
		{
			yield return response;
		}
	}

	/// <inheritdoc />
	public async Task<ApiResponse<KnowledgeboxFindResults>> FindAsync(FindRequest request, CancellationToken cancellationToken = default)
	{
		return await ExecuteHttpRequestAsync<KnowledgeboxFindResults>(
			async () => await _httpClient.PostAsJsonAsync($"{_baseUrl}/kb/{_knowledgeBoxId}/find", request, _jsonOptions, cancellationToken),
			nameof(FindAsync),
			cancellationToken,
			$"for knowledge box {_knowledgeBoxId} with query: {request.Query}");
	}

	/// <inheritdoc />
	public async Task<ApiResponse<KnowledgeboxSearchResults>> CatalogAsync(CatalogRequest request, CancellationToken cancellationToken = default)
	{
		return await ExecuteHttpRequestAsync<KnowledgeboxSearchResults>(
			async () => await _httpClient.PostAsJsonAsync($"{_baseUrl}/kb/{_knowledgeBoxId}/catalog", request, _jsonOptions, cancellationToken),
			nameof(CatalogAsync),
			cancellationToken,
			$"for knowledge box {_knowledgeBoxId}");
	}

	/// <inheritdoc />
	public async Task<ApiResponse<KnowledgeboxSuggestResults>> SuggestAsync(
		string query,
		string[]? fields = null,
		string[]? filters = null,
		string[]? faceted = null,
		DateTime? rangeCreationStart = null,
		DateTime? rangeCreationEnd = null,
		DateTime? rangeModificationStart = null,
		DateTime? rangeModificationEnd = null,
		SuggestOptions[]? features = null,
		ResourceProperties[]? show = null,
		FieldTypeName[]? fieldType = null,
		bool? debug = null,
		bool? highlight = null,
		bool? showHidden = null,
		CancellationToken cancellationToken = default)
	{
		return await ExecuteHttpRequestAsync<KnowledgeboxSuggestResults>(
			async () =>
			{
				var parameters = new Dictionary<string, object?>
				{
					["query"] = query,
				};

				if (fields != null && fields.Length > 0) parameters["fields"] = fields;
				if (filters != null && filters.Length > 0) parameters["filters"] = filters;
				if (faceted != null && faceted.Length > 0) parameters["faceted"] = faceted;
				if (rangeCreationStart != null) parameters["range_creation_start"] = rangeCreationStart!.Value.ToString("o");
				if (rangeCreationEnd != null) parameters["range_creation_end"] = rangeCreationEnd!.Value.ToString("o");
				if (rangeModificationStart != null) parameters["range_modification_start"] = rangeModificationStart!.Value.ToString("o");
				if (rangeModificationEnd != null) parameters["range_modification_end"] = rangeModificationEnd!.Value.ToString("o");
				if (features != null && features.Length > 0) parameters["features"] = features;
				if (show != null && show.Length > 0) parameters["show"] = show;
				if (fieldType != null && fieldType.Length > 0) parameters["field_type"] = fieldType;
				if (debug != null) parameters["debug"] = debug.ToString();
				if (highlight != null) parameters["highlight"] = highlight.ToString();
				if (showHidden != null) parameters["show_hidden"] = showHidden.ToString();

				var queryString = HttpUtilityHelper.BuildQueryStringWithArrays(parameters);
				var url = HttpUtilityHelper.AppendQueryString($"{_baseUrl}/kb/{_knowledgeBoxId}/suggest", queryString);
				return await _httpClient.GetAsync(url, cancellationToken);
				},
				nameof(SuggestAsync),
				cancellationToken,
				$"for knowledge box {_knowledgeBoxId} with query: {query}");
	}

	/// <inheritdoc />
	public async Task<ApiResponse<SummarizedResponse>> SummarizeAsync(SummarizeRequest request, bool showConsumption = false, CancellationToken cancellationToken = default)
	{
		return await ExecuteHttpRequestAsync<SummarizedResponse>(
			async () =>
			{
				using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/kb/{_knowledgeBoxId}/summarize");
				httpRequest.Content = JsonContent.Create(request, options: _jsonOptions);
				if (showConsumption)
				{
					httpRequest.AddBooleanHeader("x-show-consumption", true);
				}
				return await _httpClient.SendAsync(httpRequest, cancellationToken);
			},
			nameof(SummarizeAsync),
			cancellationToken,
			$"for knowledge box {_knowledgeBoxId}");
	}

	/// <inheritdoc />
	public async Task<ApiResponse<GraphSearchResponse>> GraphSearchAsync(GraphSearchRequest request, CancellationToken cancellationToken = default)
	{
		return await ExecuteHttpRequestAsync<GraphSearchResponse>(
			async () => await _httpClient.PostAsJsonAsync($"{_baseUrl}/kb/{_knowledgeBoxId}/graph", request, _jsonOptions, cancellationToken),
			nameof(GraphSearchAsync),
			cancellationToken,
			$"for knowledge box {_knowledgeBoxId}");
	}

	/// <inheritdoc />
	public async Task<ApiResponse<GraphNodesSearchResponse>> GraphNodesSearchAsync(GraphNodesSearchRequest request, CancellationToken cancellationToken = default)
	{
		return await ExecuteHttpRequestAsync<GraphNodesSearchResponse>(
			async () => await _httpClient.PostAsJsonAsync($"{_baseUrl}/kb/{_knowledgeBoxId}/graph/nodes", request, _jsonOptions, cancellationToken),
			nameof(GraphNodesSearchAsync),
			cancellationToken,
			$"for knowledge box {_knowledgeBoxId}");
	}

    /// <inheritdoc />
    public async Task<ApiResponse<GraphRelationsSearchResponse>> GraphRelationsSearchAsync(GraphRelationsSearchRequest request, CancellationToken cancellationToken = default)
    {
        return await ExecuteHttpRequestAsync<GraphRelationsSearchResponse>(
            async () => await _httpClient.PostAsJsonAsync($"{_baseUrl}/kb/{_knowledgeBoxId}/graph/relations", request, _jsonOptions, cancellationToken),
            nameof(GraphRelationsSearchAsync),
            cancellationToken,
            $"for knowledge box {_knowledgeBoxId}");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<object>> SendFeedbackAsync(FeedbackRequest request, CancellationToken cancellationToken = default)
    {
        return await ExecuteHttpRequestAsync<object>(
            async () => await _httpClient.PostAsJsonAsync($"{_baseUrl}/kb/{_knowledgeBoxId}/feedback", request, _jsonOptions, cancellationToken),
            nameof(SendFeedbackAsync),
            cancellationToken,
            $"for knowledge box {_knowledgeBoxId}");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<KnowledgeboxSearchResults>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        return await ExecuteHttpRequestAsync<KnowledgeboxSearchResults>(
            async () => await _httpClient.PostAsJsonAsync($"{_baseUrl}/kb/{_knowledgeBoxId}/search", request, _jsonOptions, cancellationToken),
            nameof(SearchAsync),
            cancellationToken,
            $"for knowledge box {_knowledgeBoxId}");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<SyncAskResponse>> AskResourceAsync(string resourceId, AskRequest request, bool showConsumption = false, CancellationToken cancellationToken = default)
    {
        return await ExecuteHttpRequestAsync<SyncAskResponse>(
            async () =>
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/kb/{_knowledgeBoxId}/resource/{resourceId}/ask");
                httpRequest.AddBooleanHeader("x-synchronous", true);
                httpRequest.Content = JsonContent.Create(request, options: _jsonOptions);
                if (showConsumption)
                {
                    httpRequest.AddBooleanHeader("x-show-consumption", true);
                }
                return await _httpClient.SendAsync(httpRequest, cancellationToken);
            },
            nameof(AskResourceAsync),
            cancellationToken,
            $"for resource {resourceId} in knowledge box {_knowledgeBoxId}");
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<SyncAskResponseUpdate> AskResourceStreamAsync(string resourceId, AskRequest request, bool showConsumption = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting AskResourceStreamAsync for resource {ResourceId} in knowledge box {KnowledgeBoxId}", resourceId, _knowledgeBoxId);
        
        HttpResponseMessage response;
        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/kb/{_knowledgeBoxId}/resource/{resourceId}/ask");
            httpRequest.Content = JsonContent.Create(request, options: _jsonOptions);
            if (showConsumption)
            {
                httpRequest.AddBooleanHeader("x-show-consumption", true);
            }
            response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "AskResourceStreamAsync HTTP request failed for resource {ResourceId}", resourceId);
            throw;
        }
        catch (Exception ex)
        {
        _logger.LogError(ex, "AskResourceStreamAsync unexpected error during request for resource {ResourceId}", resourceId);
            throw;
        }

        string? learningId = null;
        // Get value for the nuclia-learning-id Header if present
        if (response.Headers.TryGetValues("nuclia-learning-id", out var learningIdValues))
        {
            learningId = learningIdValues.FirstOrDefault();
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(line)) continue;

            var streamResponse = JsonSerializer.Deserialize<SyncAskResponseUpdate>(line, _jsonOptions);
            if (streamResponse != null)
            {
                streamResponse.LearningId = learningId;
                yield return streamResponse;
            }
        }

        _logger.LogInformation("AskResourceStreamAsync completed successfully for resource {ResourceId}", resourceId);
        response.Dispose();
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<SyncAskResponseUpdate> AskResourceStreamAsync(string resourceId, string query, bool showConsumption = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("AskResourceStreamAsync called with query: {Query} for resource {ResourceId}", query, resourceId);
        await foreach (var response in AskResourceStreamAsync(resourceId, new AskRequest(query), showConsumption, cancellationToken))
        {
            yield return response;
        }
    }

    /// <inheritdoc />
    public async Task<ApiResponse<SyncAskResponse>> AskResourceBySlugAsync(string slug, AskRequest request, bool showConsumption = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting AskResourceBySlugAsync for slug {Slug} in knowledge box {KnowledgeBoxId}", slug, _knowledgeBoxId);
        
        return await ExecuteHttpRequestAsync<SyncAskResponse>(
            async () =>
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/kb/{_knowledgeBoxId}/slug/{slug}/ask");
                httpRequest.Content = JsonContent.Create(request, options: _jsonOptions);
                httpRequest.AddBooleanHeader("x-synchronous", true);
                if (showConsumption)
                {
                    httpRequest.AddBooleanHeader("x-show-consumption", true);
                }
                return await _httpClient.SendAsync(httpRequest, cancellationToken);
            },
            nameof(AskResourceBySlugAsync),
            cancellationToken,
            $"for slug {slug} in knowledge box {_knowledgeBoxId}");
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<SyncAskResponseUpdate> AskResourceBySlugStreamAsync(string slug, AskRequest request, bool showConsumption = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting AskResourceBySlugStreamAsync for slug {Slug} in knowledge box {KnowledgeBoxId}", slug, _knowledgeBoxId);
        
        HttpResponseMessage response;
        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/kb/{_knowledgeBoxId}/slug/{slug}/ask");
            httpRequest.Content = JsonContent.Create(request, options: _jsonOptions);
            if (showConsumption)
            {
                httpRequest.AddBooleanHeader("x-show-consumption", true);
            }
            response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "AskResourceBySlugStreamAsync HTTP request failed for slug {Slug}", slug);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AskResourceBySlugStreamAsync unexpected error during request for slug {Slug}", slug);
            throw;
        }

        string? learningId = null;
        // Get value for the nuclia-learning-id Header if present
        if (response.Headers.TryGetValues("nuclia-learning-id", out var learningIdValues))
        {
            learningId = learningIdValues.FirstOrDefault();
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(line)) continue;

            var streamResponse = JsonSerializer.Deserialize<SyncAskResponseUpdate>(line, _jsonOptions);
            if (streamResponse != null)
            {
                streamResponse.LearningId = learningId;
                yield return streamResponse;
            }
        }

        _logger.LogInformation("AskResourceBySlugStreamAsync completed successfully for slug {Slug}", slug);
        response.Dispose();
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<SyncAskResponseUpdate> AskResourceBySlugStreamAsync(string slug, string query, bool showConsumption = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("AskResourceBySlugStreamAsync called with query: {Query} for slug {Slug}", query, slug);
        await foreach (var response in AskResourceBySlugStreamAsync(slug, new AskRequest(query), showConsumption, cancellationToken))
        {
            yield return response;
        }
    }

    /// <inheritdoc />
    public async Task<ApiResponse<JsonElement>> PredictProxyAsync(PredictProxiedEndpoints endpoint, object? requestBody = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting PredictProxyAsync for endpoint {Endpoint} in knowledge box {KnowledgeBoxId}", endpoint, _knowledgeBoxId);
        
        var endpointPath = PredictProxiedEndpointsValueConverter.ToJsonValue(endpoint);
        
        return await ExecuteHttpRequestAsync<JsonElement>(
            async () =>
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/kb/{_knowledgeBoxId}/predict/{endpointPath}");
                if (requestBody != null)
                {
                    httpRequest.Content = JsonContent.Create(requestBody, options: _jsonOptions);
                }
                return await _httpClient.SendAsync(httpRequest, cancellationToken);
            },
            nameof(PredictProxyAsync),
            cancellationToken,
            $"for endpoint {endpoint} in knowledge box {_knowledgeBoxId}");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<ResourceSearchResults>> ResourceSearchAsync(
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
        _logger.LogDebug("Starting ResourceSearchAsync for resource {ResourceId} with query {Query} in knowledge box {KnowledgeBoxId}", resourceId, query, _knowledgeBoxId);
        
        if (string.IsNullOrWhiteSpace(query))
        {
            return ApiResponse<ResourceSearchResults>.CreateError("Query parameter is required.");
        }

        return await ExecuteHttpRequestAsync<ResourceSearchResults>(
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
                var url = $"{_baseUrl}/kb/{_knowledgeBoxId}/resource/{resourceId}/search";
                var fullUrl = HttpUtilityHelper.AppendQueryString(url, queryString);
                return await _httpClient.GetAsync(fullUrl, cancellationToken);
            },
            nameof(ResourceSearchAsync),
            cancellationToken,
            $"for resource {resourceId} with query '{query}' in knowledge box {_knowledgeBoxId}");
    }
}
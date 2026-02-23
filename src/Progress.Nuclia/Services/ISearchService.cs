using Progress.Nuclia.Model;
using Progress.Nuclia.Model.Streaming;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Progress.Nuclia.Services;

/// <summary>
/// Interface for Search operations
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Ask a question to the Knowledge Box and get a synchronous response
    /// </summary>
    /// <param name="request">Ask request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronous ask response</returns>
    Task<ApiResponse<SyncAskResponse>> AskAsync(AskRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ask a question to the Knowledge Box and get a typed synchronous response.
    /// Automatically generates a JSON schema from type T and deserializes the structured response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the answer into</typeparam>
    /// <param name="request">Ask request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Typed synchronous ask response</returns>
    Task<ApiResponse<T>> AskAsync<T>(AskRequest request, CancellationToken cancellationToken = default);

    /// <summary>
	/// Ask a question to the Knowledge Box and get a streaming response
	/// </summary>
	/// <param name="request">Ask request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Async enumerable of streaming responses</returns>
	IAsyncEnumerable<SyncAskResponseUpdate> AskStreamAsync(AskRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ask a question to the Knowledge Box and get a streaming response
    /// </summary>
    /// <param name="query">Query with default Ask request options.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async enumerable of streaming responses</returns>
    IAsyncEnumerable<SyncAskResponseUpdate> AskStreamAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Find resources in the Knowledge Box using POST and a JSON <see cref="FindRequest"/> body.
    /// </summary>
    /// <param name="request">Find request with search parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Knowledge box find results with detailed paragraph information</returns>
    Task<ApiResponse<KnowledgeboxFindResults>> FindAsync(FindRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// List resources in the Knowledge Box using POST and a JSON <see cref="CatalogRequest"/> body.
    /// </summary>
    /// <param name="request">Catalog request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Knowledge box search results</returns>
    Task<ApiResponse<KnowledgeboxSearchResults>> CatalogAsync(CatalogRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Suggest endpoint to retrieve paragraph and entity suggestions for a query
    /// </summary>
    /// <param name="query">Query text to get suggestions for</param>
    /// <param name="fields">Restrict search to specific field paths (e.g. a/title)</param>
    /// <param name="filters">Filter expressions</param>
    /// <param name="faceted">Facet expressions to calculate</param>
    /// <param name="rangeCreationStart">Filter resources created after or on this date (UTC)</param>
    /// <param name="rangeCreationEnd">Filter resources created before or on this date (UTC)</param>
    /// <param name="rangeModificationStart">Filter resources modified after or on this date (UTC)</param>
    /// <param name="rangeModificationEnd">Filter resources modified before or on this date (UTC)</param>
    /// <param name="features">Enabled suggest features (defaults paragraph,entities)</param>
    /// <param name="show">Resource metadata types to serialize</param>
    /// <param name="fieldType">Field types to return</param>
    /// <param name="debug">Include extra debugging metadata</param>
    /// <param name="highlight">Highlight query terms in results</param>
    /// <param name="showHidden">Include hidden resources</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Suggestion results (paragraphs/entities)</returns>
    Task<ApiResponse<KnowledgeboxSuggestResults>> SuggestAsync(
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Summarize a set of resources.
    /// </summary>
    /// <param name="request">Summarize request containing resource IDs/slugs and options.</param>
    /// <param name="showConsumption">If true adds x-show-consumption header to include token consumption.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Summarized response.</returns>
    Task<ApiResponse<SummarizedResponse>> SummarizeAsync(SummarizeRequest request, bool showConsumption = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search Knowledge Box graph (triplets vertex-edge-vertex).
    /// </summary>
    Task<ApiResponse<GraphSearchResponse>> GraphSearchAsync(GraphSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search Knowledge Box graph nodes (vertices).
    /// </summary>
    Task<ApiResponse<GraphNodesSearchResponse>> GraphNodesSearchAsync(GraphNodesSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search Knowledge Box graph relations (edges).
    /// </summary>
    Task<ApiResponse<GraphRelationsSearchResponse>> GraphRelationsSearchAsync(GraphRelationsSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send feedback for a search operation in the Knowledge Box
    /// </summary>
    /// <param name="request">Feedback request containing the operation identifier, rating, and task type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API response indicating success or failure of feedback submission</returns>
    Task<ApiResponse<object>> SendFeedbackAsync(FeedbackRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search Knowledge Box and retrieve separate results for documents, paragraphs, and sentences
    /// </summary>
    /// <param name="request">Search request with search parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Knowledge box search results with documents, paragraphs, and sentences</returns>
    Task<ApiResponse<KnowledgeboxSearchResults>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ask a question to a specific resource and get a synchronous response
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="request">Ask request</param>
    /// <param name="showConsumption">If true adds x-show-consumption header to include token consumption</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronous ask response</returns>
    Task<ApiResponse<SyncAskResponse>> AskResourceAsync(string resourceId, AskRequest request, bool showConsumption = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ask a question to a specific resource and get a streaming response
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="request">Ask request</param>
    /// <param name="showConsumption">If true adds x-show-consumption header to include token consumption</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async enumerable of streaming responses</returns>
    IAsyncEnumerable<SyncAskResponseUpdate> AskResourceStreamAsync(string resourceId, AskRequest request, bool showConsumption = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ask a question to a specific resource and get a streaming response
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="query">Query with default Ask request options</param>
    /// <param name="showConsumption">If true adds x-show-consumption header to include token consumption</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async enumerable of streaming responses</returns>
    IAsyncEnumerable<SyncAskResponseUpdate> AskResourceStreamAsync(string resourceId, string query, bool showConsumption = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ask a question about a specific resource by slug in the knowledge box synchronously.
    /// </summary>
    /// <param name="slug">The slug of the resource to ask about.</param>
    /// <param name="request">The ask request containing the question and parameters.</param>
    /// <param name="showConsumption">Whether to include consumption information in the response.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A response containing the answer and related information.</returns>
    Task<ApiResponse<SyncAskResponse>> AskResourceBySlugAsync(string slug, AskRequest request, bool showConsumption = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ask a question about a specific resource by slug in the knowledge box with streaming response.
    /// </summary>
    /// <param name="slug">The slug of the resource to ask about.</param>
    /// <param name="request">The ask request containing the question and parameters.</param>
    /// <param name="showConsumption">Whether to include consumption information in the response.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>An async enumerable of ask response updates.</returns>
    IAsyncEnumerable<SyncAskResponseUpdate> AskResourceBySlugStreamAsync(string slug, AskRequest request, bool showConsumption = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ask a question about a specific resource by slug in the knowledge box with streaming response.
    /// </summary>
    /// <param name="slug">The slug of the resource to ask about.</param>
    /// <param name="query">The question to ask about the resource.</param>
    /// <param name="showConsumption">Whether to include consumption information in the response.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>An async enumerable of ask response updates.</returns>
    IAsyncEnumerable<SyncAskResponseUpdate> AskResourceBySlugStreamAsync(string slug, string query, bool showConsumption = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Proxy endpoint that forwards requests to the Predict API.
    /// It adds the Knowledge Box configuration settings as headers to the predict API request.
    /// </summary>
    /// <param name="endpoint">The predict endpoint to proxy to.</param>
    /// <param name="requestBody">The request body to send to the predict API.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A response containing the predict API result.</returns>
    Task<ApiResponse<JsonElement>> PredictProxyAsync(PredictProxiedEndpoints endpoint, object? requestBody = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search within a specific resource by ID.
    /// </summary>
    /// <param name="resourceId">The ID of the resource to search within.</param>
    /// <param name="query">The search query.</param>
    /// <param name="fields">The list of fields to search in.</param>
    /// <param name="filters">The list of filters to apply.</param>
    /// <param name="faceted">The list of facets to calculate.</param>
    /// <param name="filterExpression">Filter expression to apply.</param>
    /// <param name="sortField">Field to sort results with.</param>
    /// <param name="sortOrder">Order to sort results with.</param>
    /// <param name="topK">The number of results to return.</param>
    /// <param name="rangeCreationStart">Resources created before this date will be filtered out.</param>
    /// <param name="rangeCreationEnd">Resources created after this date will be filtered out.</param>
    /// <param name="rangeModificationStart">Resources modified before this date will be filtered out.</param>
    /// <param name="rangeModificationEnd">Resources modified after this date will be filtered out.</param>
    /// <param name="highlight">Whether to include highlighting information.</param>
    /// <param name="debug">Whether to include debug information.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A response containing the search results within the resource.</returns>
    Task<ApiResponse<ResourceSearchResults>> ResourceSearchAsync(
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
        CancellationToken cancellationToken = default);
}

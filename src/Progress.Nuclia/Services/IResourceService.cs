using Progress.Nuclia.Model;

namespace Progress.Nuclia.Services;

/// <summary>
/// Interface for Resource operations
/// </summary>
public interface IResourceService
{
    /// <summary>
    /// Retrieve a paginated list of resources from the Knowledge Box
    /// </summary>
    /// <param name="page">The page number to retrieve (zero-indexed)</param>
    /// <param name="size">The number of resources to return per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list of resources with pagination metadata</returns>
    Task<ApiResponse<ResourceList>> ListResourcesAsync(int? page = default, int? size = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a resource by its ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="show">Properties to include in the response</param>
    /// <param name="fieldType">Properties to include in the response</param>
    /// <param name="extracted">Properties to include in the response</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource information</returns>
    Task<ApiResponse<NucliadbModelsResourceResource>> GetResourceByIdAsync(
        string resourceId,
        ResourceProperties[]? show = null,
        FieldTypeName[]? fieldType = null,
        ExtractedDataTypeName[]? extracted = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a resource by its slug
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="show">Properties to include in the response</param>
    /// <param name="fieldType">Field types to include</param>
    /// <param name="extracted">Extracted data types to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource information</returns>
    Task<ApiResponse<NucliadbModelsResourceResource>> GetResourceBySlugAsync(
        string resourceSlug,
        ResourceProperties[]? show = null,
        FieldTypeName[]? fieldType = null,
        ExtractedDataTypeName[]? extracted = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a resource by its ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    Task<ApiResponse<bool>> DeleteResourceByIdAsync(string resourceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a resource by its ID
    /// </summary>
    /// <param name="resourceSlug">Resource Slug</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    Task<ApiResponse<bool>> DeleteResourceBySlugAsync(string resourceSlug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Download field binary field by ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="inline">Whether to display the file inline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Binary file content</returns>
    Task<ApiResponse<byte[]>> DownloadFieldFileAsync(string resourceId, string fieldId, bool inline = false, CancellationToken cancellationToken = default);  
    
    /// <summary>
    /// Download field binary field by URL
    /// </summary>
    /// <param name="resourceUrl">Complete resource URL path</param>
    /// <param name="inline">Whether to display the file inline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Binary file content</returns>
    Task<ApiResponse<byte[]>> DownloadFieldFileAsync(string resourceUrl, bool inline = false, CancellationToken cancellationToken = default);


    /// <summary>
    /// Create a new resource in the Knowledge Box
    /// </summary>
    /// <param name="request">Resource creation request</param>
    /// <param name="skipStore">If set to true, file fields will not be saved in the blob storage</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource creation response</returns>
    Task<ApiResponse<ResourceCreated>> CreateResourceAsync(CreateResourcePayload request, bool skipStore = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reindex a resource by its ID. Triggers a background reindex operation.
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="reindexVectors">Whether to reindex vectors as well (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status (true if accepted)</returns>
    Task<ApiResponse<bool>> ReindexResourceByIdAsync(string resourceId, bool? reindexVectors = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reindex a resource by its slug. Triggers a background reindex operation.
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="reindexVectors">Whether to reindex vectors as well (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status (true if accepted)</returns>
    Task<ApiResponse<bool>> ReindexResourceBySlugAsync(string resourceSlug, bool? reindexVectors = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reprocess a resource by its ID. Returns ResourceUpdated information on 202 Accepted.
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="resetTitle">Reset the title so computed titles are set after processing.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource update status</returns>
    Task<ApiResponse<ResourceUpdated>> ReprocessResourceByIdAsync(string resourceId, bool? resetTitle = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reprocess a resource by its slug. Returns ResourceUpdated information on 202 Accepted.
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="resetTitle">Reset the title so computed titles are set after processing.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource update status</returns>
    Task<ApiResponse<ResourceUpdated>> ReprocessResourceBySlugAsync(string resourceSlug, bool? resetTitle = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Modify a resource by its slug using a PATCH operation.
    /// </summary>
    /// <param name="resourceSlug">The slug of the resource to modify.</param>
    /// <param name="payload">The update payload containing the fields to modify.</param>
    /// <param name="skipStore">If set to true, file fields will not be saved in the blob storage.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A response containing the resource update information.</returns>
    Task<ApiResponse<ResourceUpdated>> ModifyResourceBySlugAsync(string resourceSlug, UpdateResourcePayload payload, bool skipStore = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Modify a resource by its ID using a PATCH operation.
    /// </summary>
    /// <param name="resourceId">The ID of the resource to modify.</param>
    /// <param name="payload">The update payload containing the fields to modify.</param>
    /// <param name="skipStore">If set to true, file fields will not be saved in the blob storage.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A response containing the resource update information.</returns>
    Task<ApiResponse<ResourceUpdated>> ModifyResourceByIdAsync(string resourceId, UpdateResourcePayload payload, bool skipStore = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Run ingestion agents for a resource by its ID.
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="request">Agents execution payload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Agents execution response</returns>
    Task<ApiResponse<ResourceAgentsResponse>> RunAgentsOnResourceByIdAsync(string resourceId, ResourceAgentsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search within a single resource by its ID.
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="query">Search query (required)</param>
    /// <param name="fields">Fields to search in</param>
    /// <param name="filters">Filters to apply</param>
    /// <param name="faceted">Faceted fields to calculate</param>
    /// <param name="filterExpression">Advanced filter expression</param>
    /// <param name="sortField">Field to sort results</param>
    /// <param name="sortOrder">Sort order</param>
    /// <param name="topK">Maximum results per data type</param>
    /// <param name="rangeCreationStart">Creation range start</param>
    /// <param name="rangeCreationEnd">Creation range end</param>
    /// <param name="rangeModificationStart">Modification range start</param>
    /// <param name="rangeModificationEnd">Modification range end</param>
    /// <param name="highlight">Enable highlight in results</param>
    /// <param name="debug">Enable debug metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource search results</returns>
    Task<ApiResponse<ResourceSearchResults>> SearchWithinResourceByIdAsync(
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

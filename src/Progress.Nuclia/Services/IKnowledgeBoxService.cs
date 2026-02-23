using Progress.Nuclia.Model;

namespace Progress.Nuclia.Services;

/// <summary>
/// Interface for Knowledge Box operations
/// </summary>
public interface IKnowledgeBoxService
{
    /// <summary>
    /// Get a Knowledge Box by its ID
    /// </summary>
    /// <param name="knowledgeBoxId">Knowledge Box ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Knowledge Box information</returns>
    Task<ApiResponse<KnowledgeBoxObj>> GetKnowledgeBoxAsync(string knowledgeBoxId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a Knowledge Box by its slug
    /// </summary>
    /// <param name="slug">Knowledge Box slug</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Knowledge Box information</returns>
    Task<ApiResponse<KnowledgeBoxObj>> GetKnowledgeBoxBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get counters summary for a Knowledge Box
    /// </summary>
    Task<ApiResponse<KnowledgeboxCounters>> GetKnowledgeBoxCountersAsync(string knowledgeBoxId, bool? debug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get current models configuration of a Knowledge Box
    /// </summary>
    Task<ApiResponse<Dictionary<string, object>>> GetKnowledgeBoxConfigurationAsync(string knowledgeBoxId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Patch (partial update) models configuration of a Knowledge Box
    /// Returns Success true if 204
    /// </summary>
    Task<ApiResponse<bool>> PatchKnowledgeBoxConfigurationAsync(string knowledgeBoxId, Dictionary<string, object> configurationPatch, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set (create/replace) models configuration of a Knowledge Box
    /// Returns Success true if 204
    /// </summary>
    Task<ApiResponse<bool>> SetKnowledgeBoxConfigurationAsync(string knowledgeBoxId, Dictionary<string, object> configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Download an export for a Knowledge Box (raw bytes)
    /// </summary>
    Task<ApiResponse<byte[]>> DownloadKnowledgeBoxExportAsync(string knowledgeBoxId, string exportId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get status of a Knowledge Box export
    /// </summary>
    Task<ApiResponse<StatusResponse>> GetKnowledgeBoxExportStatusAsync(string knowledgeBoxId, string exportId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get status of a Knowledge Box import
    /// </summary>
    Task<ApiResponse<StatusResponse>> GetKnowledgeBoxImportStatusAsync(string knowledgeBoxId, string importId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload a file to a Knowledge Box (simplified single file upload)
    /// </summary>
    Task<ApiResponse<ResourceFileUploaded>> UploadKnowledgeBoxFileAsync(string knowledgeBoxId, Stream fileStream, string fileName, string? splitStrategy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get learning configuration JSON schema
    /// </summary>
    Task<ApiResponse<Dictionary<string, object>>> GetLearningConfigurationSchemaAsync(CancellationToken cancellationToken = default);
}

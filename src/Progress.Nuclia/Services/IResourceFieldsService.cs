using Progress.Nuclia.Model;

namespace Progress.Nuclia.Services;

/// <summary>
/// Interface for Resource Field operations
/// </summary>
public interface IResourceFieldsService
{
    // ===== Conversation Field Operations =====

    /// <summary>
    /// Add a conversation field to a resource by ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="conversationField">Conversation field data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource field added response</returns>
    Task<ApiResponse<ResourceFieldAdded>> AddConversationFieldByIdAsync(
        string resourceId,
        string fieldId,
        InputConversationField conversationField,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a conversation field to a resource by slug
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="conversationField">Conversation field data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource field added response</returns>
    Task<ApiResponse<ResourceFieldAdded>> AddConversationFieldBySlugAsync(
        string resourceSlug,
        string fieldId,
        InputConversationField conversationField,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Append messages to an existing conversation field by resource ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="messages">Messages to append</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    Task<ApiResponse<ResourceFieldAdded>> AppendMessagesToConversationFieldByIdAsync(
        string resourceId,
        string fieldId,
        List<InputMessage> messages,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Append messages to an existing conversation field by resource slug
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="messages">Messages to append</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    Task<ApiResponse<ResourceFieldAdded>> AppendMessagesToConversationFieldBySlugAsync(
        string resourceSlug,
        string fieldId,
        List<InputMessage> messages,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a conversation field attachment by resource ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="messageId">Message ID</param>
    /// <param name="fileNum">File number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Binary file content</returns>
    Task<ApiResponse<byte[]>> DownloadConversationAttachmentByIdAsync(
        string resourceId,
        string fieldId,
        string messageId,
        int fileNum,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a conversation field attachment by resource slug
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="messageId">Message ID</param>
    /// <param name="fileNum">File number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Binary file content</returns>
    Task<ApiResponse<byte[]>> DownloadConversationAttachmentBySlugAsync(
        string resourceSlug,
        string fieldId,
        string messageId,
        int fileNum,
        CancellationToken cancellationToken = default);

    // ===== File Field Operations =====

    /// <summary>
    /// Add a file field to a resource by ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="fileField">File field data</param>
    /// <param name="skipStore">If true, file will not be saved in blob storage</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource field added response</returns>
    Task<ApiResponse<ResourceFieldAdded>> AddFileFieldByIdAsync(
        string resourceId,
        string fieldId,
        FileField fileField,
        bool skipStore = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a file field to a resource by slug
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="fileField">File field data</param>
    /// <param name="skipStore">If true, file will not be saved in blob storage</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource field added response</returns>
    Task<ApiResponse<ResourceFieldAdded>> AddFileFieldBySlugAsync(
        string resourceSlug,
        string fieldId,
        FileField fileField,
        bool skipStore = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload a binary file to a resource field by ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="fileContent">File content as byte array</param>
    /// <param name="filename">Name of the file being uploaded</param>
    /// <param name="password">Password if file is password protected</param>
    /// <param name="language">Language of the file content</param>
    /// <param name="md5">MD5 hash of the file</param>
    /// <param name="extractStrategy">Extract strategy to use</param>
    /// <param name="splitStrategy">Split strategy to use</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource file uploaded response</returns>
    Task<ApiResponse<ResourceFileUploaded>> UploadFileByIdAsync(
        string resourceId,
        string fieldId,
        byte[] fileContent,
        string? filename = null,
        string? password = null,
        string? language = null,
        string? md5 = null,
        string? extractStrategy = null,
        string? splitStrategy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload a binary file to a resource field by slug
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="fileContent">File content as byte array</param>
    /// <param name="filename">Name of the file being uploaded</param>
    /// <param name="password">Password if file is password protected</param>
    /// <param name="language">Language of the file content</param>
    /// <param name="md5">MD5 hash of the file</param>
    /// <param name="extractStrategy">Extract strategy to use</param>
    /// <param name="splitStrategy">Split strategy to use</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource file uploaded response</returns>
    Task<ApiResponse<ResourceFileUploaded>> UploadFileBySlugAsync(
        string resourceSlug,
        string fieldId,
        byte[] fileContent,
        string? filename = null,
        string? password = null,
        string? language = null,
        string? md5 = null,
        string? extractStrategy = null,
        string? splitStrategy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a file field by resource ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="inline">Whether to display the file inline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Binary file content</returns>
    Task<ApiResponse<byte[]>> DownloadFileFieldByIdAsync(
        string resourceId,
        string fieldId,
        bool inline = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a file field by resource slug
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="inline">Whether to display the file inline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Binary file content</returns>
    Task<ApiResponse<byte[]>> DownloadFileFieldBySlugAsync(
        string resourceSlug,
        string fieldId,
        bool inline = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reprocess a file field by resource ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="resetTitle">Whether to reset the title</param>
    /// <param name="filePassword">Password for the file if protected</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    /// <remarks>
    /// Note: The Nuclia API only supports reprocessing by resource ID, not by slug.
    /// </remarks>
    Task<ApiResponse<bool>> ReprocessFileFieldByIdAsync(
        string resourceId,
        string fieldId,
        bool? resetTitle = null,
        string? filePassword = null,
        CancellationToken cancellationToken = default);

    // ===== Link Field Operations =====

    /// <summary>
    /// Add a link field to a resource by ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="linkField">Link field data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource field added response</returns>
    Task<ApiResponse<ResourceFieldAdded>> AddLinkFieldByIdAsync(
        string resourceId,
        string fieldId,
        LinkField linkField,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a link field to a resource by slug
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="linkField">Link field data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource field added response</returns>
    Task<ApiResponse<ResourceFieldAdded>> AddLinkFieldBySlugAsync(
        string resourceSlug,
        string fieldId,
        LinkField linkField,
        CancellationToken cancellationToken = default);

    // ===== Text Field Operations =====

    /// <summary>
    /// Add a text field to a resource by ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="textField">Text field data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource field added response</returns>
    Task<ApiResponse<ResourceFieldAdded>> AddTextFieldByIdAsync(
        string resourceId,
        string fieldId,
        TextField textField,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a text field to a resource by slug
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="textField">Text field data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource field added response</returns>
    Task<ApiResponse<ResourceFieldAdded>> AddTextFieldBySlugAsync(
        string resourceSlug,
        string fieldId,
        TextField textField,
        CancellationToken cancellationToken = default);

    // ===== Generic Field Operations =====

    /// <summary>
    /// Get a resource field by ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="fieldType">Field type</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="show">Properties to include in the response</param>
    /// <param name="extracted">Extracted data types to include</param>
    /// <param name="page">Page number or 'last'</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource field data</returns>
    Task<ApiResponse<ResourceField>> GetResourceFieldByIdAsync(
        string resourceId,
        FieldTypeName fieldType,
        string fieldId,
        ResourceFieldProperties[]? show = null,
        ExtractedDataTypeName[]? extracted = null,
        string? page = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a resource field by slug
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="fieldType">Field type</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="show">Properties to include in the response</param>
    /// <param name="extracted">Extracted data types to include</param>
    /// <param name="page">Page number or 'last'</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource field data</returns>
    Task<ApiResponse<ResourceField>> GetResourceFieldBySlugAsync(
        string resourceSlug,
        FieldTypeName fieldType,
        string fieldId,
        ResourceFieldProperties[]? show = null,
        ExtractedDataTypeName[]? extracted = null,
        string? page = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a resource field by ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="fieldType">Field type</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    Task<ApiResponse<bool>> DeleteResourceFieldByIdAsync(
        string resourceId,
        FieldTypeName fieldType,
        string fieldId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a resource field by slug
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="fieldType">Field type</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    Task<ApiResponse<bool>> DeleteResourceFieldBySlugAsync(
        string resourceSlug,
        FieldTypeName fieldType,
        string fieldId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download an extracted binary file by resource ID
    /// </summary>
    /// <param name="resourceId">Resource ID</param>
    /// <param name="fieldType">Field type</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="downloadField">Download field identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Binary file content</returns>
    Task<ApiResponse<byte[]>> DownloadExtractedFileByIdAsync(
        string resourceId,
        FieldTypeName fieldType,
        string fieldId,
        string downloadField,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download an extracted binary file by resource slug
    /// </summary>
    /// <param name="resourceSlug">Resource slug</param>
    /// <param name="fieldType">Field type</param>
    /// <param name="fieldId">Field ID</param>
    /// <param name="downloadField">Download field identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Binary file content</returns>
    Task<ApiResponse<byte[]>> DownloadExtractedFileBySlugAsync(
        string resourceSlug,
        FieldTypeName fieldType,
        string fieldId,
        string downloadField,
        CancellationToken cancellationToken = default);
}

using Microsoft.Extensions.Logging;
using Progress.Nuclia.Model;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Progress.Nuclia.Services;

/// <summary>
/// Base class for service implementations providing common HTTP request handling and error management
/// </summary>
internal abstract class BaseService
{
    protected readonly HttpClient _httpClient;
    protected readonly string _baseUrl;
    protected readonly JsonSerializerOptions _jsonOptions;
    protected readonly ILogger _logger;

    protected BaseService(
        HttpClient httpClient,
        string baseUrl,
        JsonSerializerOptions jsonOptions,
        ILogger logger)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
        _jsonOptions = jsonOptions;
        _logger = logger;
    }

    /// <summary>
    /// Executes an HTTP request with standardized error handling, validation error processing, and logging.
    /// </summary>
    /// <typeparam name="T">The expected response type</typeparam>
    /// <param name="httpOperation">The HTTP operation to execute</param>
    /// <param name="operationName">Name of the operation for logging purposes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="additionalLogContext">Optional additional context to include in debug logs</param>
    /// <returns>ApiResponse wrapping the result or error information</returns>
    protected async Task<ApiResponse<T>> ExecuteHttpRequestAsync<T>(
        Func<Task<HttpResponseMessage>> httpOperation,
        string operationName,
        CancellationToken cancellationToken,
        string? additionalLogContext = null)
    {
        var logMessage = string.IsNullOrEmpty(additionalLogContext)
            ? $"Starting {operationName}"
            : $"Starting {operationName} {additionalLogContext}";

        _logger.LogDebug(logMessage);

        try
        {
            var response = await httpOperation();

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = await response.Content.ReadFromJsonAsync<HTTPValidationError>(_jsonOptions, cancellationToken);
                _logger.LogWarning("{OperationName} validation error: {@ValidationError}", operationName, validationError);
                return ApiResponse<T>.CreateValidationError(validationError!);
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken);
            _logger.LogInformation("{OperationName} completed successfully", operationName);
            return ApiResponse<T>.CreateSuccess(result!);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{OperationName} HTTP request failed", operationName);
            return ApiResponse<T>.CreateError($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{OperationName} unexpected error", operationName);
            return ApiResponse<T>.CreateError($"Unexpected error: {ex.Message}");
        }
    }
}

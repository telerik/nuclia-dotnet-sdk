using System.Text.Json.Serialization;

namespace Progress.Nuclia.Model;

/// <summary>
/// Base API response wrapper
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// Data payload returned on success; null if operation failed.
    /// </summary>
    public T? Data { get; set; }
    /// <summary>
    /// Indicates whether the request completed successfully.
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// Error message describing failure; null on success.
    /// </summary>
    public string? Error { get; set; }
    /// <summary>
    /// Validation error details when input validation fails.
    /// </summary>
    public HTTPValidationError? ValidationError { get; set; }

    /// <summary>
    /// Factory helper to create a successful response wrapper.
    /// </summary>
    /// <param name="data">The successful result payload.</param>
    public static ApiResponse<T> CreateSuccess(T data)
    {
        return new ApiResponse<T>
        {
            Data = data,
            Success = true
        };
    }

    /// <summary>
    /// Factory helper to create an error response wrapper.
    /// </summary>
    /// <param name="error">Error message describing the failure.</param>
    public static ApiResponse<T> CreateError(string error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = error
        };
    }

    /// <summary>
    /// Factory helper to create a validation error response wrapper.
    /// </summary>
    /// <param name="validationError">Validation error detail structure.</param>
    public static ApiResponse<T> CreateValidationError(HTTPValidationError validationError)
    {
        return new ApiResponse<T>
        {
            Success = false,
            ValidationError = validationError
        };
    }
}

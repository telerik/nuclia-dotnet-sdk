using System.Text.Json;

namespace Progress.Nuclia.Services;

/// <summary>
/// Helper class for building HTTP requests with common patterns
/// </summary>
internal static class HttpRequestHelper
{
    /// <summary>
    /// Converts an enum value to lowercase string for use in query parameters or headers
    /// </summary>
    /// <param name="enumValue">The enum value to convert</param>
    /// <returns>Lowercase string representation of the enum</returns>
    public static string ToLowerString(this Enum enumValue)
    {
        return enumValue.ToString().ToLowerInvariant();
    }

    /// <summary>
    /// Converts a boolean value to lowercase string for use in query parameters or headers
    /// </summary>
    /// <param name="boolValue">The boolean value to convert</param>
    /// <returns>Lowercase string representation of the boolean</returns>
    public static string ToLowerString(this bool boolValue)
    {
        return boolValue.ToString().ToLowerInvariant();
    }

    /// <summary>
    /// Converts an array of enums to lowercase strings for use in query parameters
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    /// <param name="enumArray">The array of enum values</param>
    /// <returns>Array of lowercase string representations</returns>
    public static string[] ToLowerStrings<T>(this T[] enumArray) where T : Enum
    {
        return enumArray.Select(e => e.ToLowerString()).ToArray();
    }

    /// <summary>
    /// Creates an HTTP request message with JSON content
    /// </summary>
    /// <typeparam name="T">The type of the request body</typeparam>
    /// <param name="method">HTTP method</param>
    /// <param name="url">Request URL</param>
    /// <param name="body">Request body object</param>
    /// <param name="jsonOptions">JSON serialization options</param>
    /// <returns>Configured HttpRequestMessage</returns>
    public static HttpRequestMessage CreateJsonRequest<T>(
        HttpMethod method,
        string url,
        T body,
        JsonSerializerOptions jsonOptions)
    {
        var request = new HttpRequestMessage(method, url);
        var jsonContent = JsonSerializer.Serialize(body, jsonOptions);
        request.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        return request;
    }

    /// <summary>
    /// Adds a custom header to an HTTP request message
    /// </summary>
    /// <param name="request">The HTTP request message</param>
    /// <param name="headerName">The header name</param>
    /// <param name="value">The header value</param>
    public static void AddHeader(this HttpRequestMessage request, string headerName, string value)
    {
        request.Headers.Add(headerName, value);
    }

    /// <summary>
    /// Adds a boolean custom header to an HTTP request message (converts to lowercase string)
    /// </summary>
    /// <param name="request">The HTTP request message</param>
    /// <param name="headerName">The header name</param>
    /// <param name="value">The boolean value</param>
    public static void AddBooleanHeader(this HttpRequestMessage request, string headerName, bool value)
    {
        request.Headers.Add(headerName, value.ToString().ToLowerInvariant());
    }
}

using System.Collections.Specialized;
using System.Web;

namespace Progress.Nuclia.Services;

/// <summary>
/// Helper class for building HTTP query strings
/// </summary>
internal static class HttpUtilityHelper
{
    /// <summary>
    /// Builds a query string from the provided parameters, automatically skipping null values
    /// </summary>
    /// <param name="parameters">Dictionary of parameter names and values</param>
    /// <returns>Query string (without leading '?') or empty string if no parameters</returns>
    public static string BuildQueryString(Dictionary<string, string?> parameters)
    {
        var queryParams = HttpUtility.ParseQueryString(string.Empty);

        foreach (var param in parameters)
        {
            if (param.Value != null)
            {
                queryParams.Add(param.Key, param.Value);
            }
        }

        return queryParams.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Adds multiple values for the same parameter name to a query string collection
    /// </summary>
    /// <param name="queryParams">The query parameters collection</param>
    /// <param name="paramName">The parameter name</param>
    /// <param name="values">The array of values to add</param>
    public static void AddMultipleValues(NameValueCollection queryParams, string paramName, string[] values)
    {
        foreach (var value in values)
        {
            queryParams.Add(paramName, value);
        }
    }

    /// <summary>
    /// Builds a query string with support for array parameters (multiple values for same key)
    /// </summary>
    /// <param name="parameters">Dictionary of parameter names and their values (can be arrays)</param>
    /// <returns>Query string (without leading '?') or empty string if no parameters</returns>
    public static string BuildQueryStringWithArrays(Dictionary<string, object?> parameters)
    {
        var queryParams = HttpUtility.ParseQueryString(string.Empty);

        foreach (var param in parameters)
        {
            if (param.Value == null)
                continue;

            if (param.Value is string[] stringArray)
            {
                AddMultipleValues(queryParams, param.Key, stringArray);
            }
            else if (param.Value is Array enumArray)
            {
                foreach (var item in enumArray)
                {
                    queryParams.Add(param.Key, item.ToString()!.ToLowerInvariant());
                }
            }
            else
            {
                queryParams.Add(param.Key, param.Value.ToString());
            }
        }

        return queryParams.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Appends a query string to a URL, handling existing query parameters correctly
    /// </summary>
    /// <param name="url">The base URL</param>
    /// <param name="queryString">The query string to append (without leading '?')</param>
    /// <returns>The complete URL with query string</returns>
    public static string AppendQueryString(string url, string queryString)
    {
        if (string.IsNullOrEmpty(queryString))
            return url;

        var separator = url.Contains('?') ? "&" : "?";
        return $"{url}{separator}{queryString}";
    }
}

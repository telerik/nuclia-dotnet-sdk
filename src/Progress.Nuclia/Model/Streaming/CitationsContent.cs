using System.Text.Json.Serialization;

namespace Progress.Nuclia.Model.Streaming;

/// <summary>
/// Represents citation content in a streaming response.
/// </summary>
public class CitationsContent : SyncAskResponseUpdateItem
{
    /// <summary>
    /// Gets or sets the citations dictionary mapping identifiers to citation ranges.
    /// </summary>
    [JsonPropertyName("citations")]
    public Dictionary<string, int[][]> Citations { get; set; } = new();
}


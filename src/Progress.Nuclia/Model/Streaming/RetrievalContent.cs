using System.Text.Json.Serialization;

namespace Progress.Nuclia.Model.Streaming;

/// <summary>
/// Represents the content retrieved in a streaming response.
/// </summary>
public class RetrievalContent : SyncAskResponseUpdateItem
{
    /// <summary>
    /// Gets or sets the results of the retrieval operation.
    /// </summary>
    [JsonPropertyName("results")]
    public KnowledgeboxFindResults? Results { get; set; }

    /// <summary>
    /// Gets or sets the best matches for the retrieval operation.
    /// </summary>
    [JsonPropertyName("best_matches")]
    public List<AskRetrievalMatch>? BestMatches { get; set; }
}

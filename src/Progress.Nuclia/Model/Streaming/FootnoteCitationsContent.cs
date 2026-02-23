using System.Text.Json.Serialization;

namespace Progress.Nuclia.Model.Streaming;

/// <summary>
/// Represents footnote citation content in a streaming response.
/// </summary>
public class FootnoteCitationsContent : SyncAskResponseUpdateItem
{
	/// <summary>
	/// Gets or sets the footnotes mapping identifiers to content text.
	/// </summary>
	[JsonPropertyName("footnote_to_context")]
	public Dictionary<string, string> FootnoteToContext { get; set; } = new();
}

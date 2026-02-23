using System.Text.Json.Serialization;

namespace Progress.Nuclia.Model.Streaming;

/// <summary>
/// Represents the content of an answer in a streaming response.
/// </summary>
public class AnswerContent : SyncAskResponseUpdateItem
{
	/// <summary>
	/// Gets or sets the text content of the answer.
	/// </summary>
	[JsonPropertyName("text")]
	public string? Text { get; set; }
}

/// <summary>
/// Represents the status content in a streaming response.
/// </summary>
public class StatusContent : SyncAskResponseUpdateItem
{
	/// <summary>
	/// Gets or sets the status message.
	/// </summary>
	[JsonPropertyName("status")]
	public string? Status { get; set; }
	
	/// <summary>
	/// Gets or sets the status code.
	/// </summary>
	[JsonPropertyName("code")]
	public int Code { get; set; }
}

/// <summary>
/// Represents metadata content in a streaming response.
/// </summary>
public class MetaDataContent : SyncAskResponseUpdateItem
{
	/// <summary>
	/// Gets or sets the token usage information.
	/// </summary>
	[JsonPropertyName("tokens")]
	public AskTokens? Tokens { get; set; }
	
	/// <summary>
	/// Gets or sets the timing information for processing.
	/// </summary>
	[JsonPropertyName("timings")]
	public AskTimings? Timings { get; set; }
}

/// <summary>
/// Represents augmented context content in a streaming response.
/// </summary>
public class AugmentedContextContent : SyncAskResponseUpdateItem
{
}

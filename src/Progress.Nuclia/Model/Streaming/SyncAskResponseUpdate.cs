namespace Progress.Nuclia.Model.Streaming;

/// <summary>
/// Stream response item for real-time ask responses
/// </summary>
public class SyncAskResponseUpdate
{
	/// <summary>
	/// Gets or sets the RAG content item in this stream response.
	/// </summary>
	public SyncAskResponseUpdateItem? Item { get; set; }

	/// <summary>
	/// Gets or sets the learning ID for this response, used for feedback tracking.
	/// </summary>
	public string? LearningId { get; set; }
}

using System.Text.Json.Serialization;

namespace Progress.Nuclia.Model.Streaming;

// TODO: JSON samples of AugmentedContent have not been verified yet.
/// <summary>
/// Base class for all content types that can be returned in a streaming response.
/// Serves as the polymorphic base for different types of RAG (Retrieval-Augmented Generation) content.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AnswerContent), typeDiscriminator: "answer")]
[JsonDerivedType(typeof(RetrievalContent), typeDiscriminator: "retrieval")]
[JsonDerivedType(typeof(CitationsContent), typeDiscriminator: "citations")]
[JsonDerivedType(typeof(StatusContent), typeDiscriminator: "status")]
[JsonDerivedType(typeof(MetaDataContent), typeDiscriminator: "metadata")]
[JsonDerivedType(typeof(DebugContent), typeDiscriminator: "debug")]
[JsonDerivedType(typeof(AugmentedContextContent), typeDiscriminator: "augmented_context")]
[JsonDerivedType(typeof(FootnoteCitationsContent), typeDiscriminator: "footnote_citations")]
public class SyncAskResponseUpdateItem
{
}

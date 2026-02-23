using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Progress.Nuclia.Model.Streaming;

/// <summary>
/// Represents debug information from the Syntha API response
/// </summary>
public class DebugContent : SyncAskResponseUpdateItem
{
    /// <summary>
    /// Debug metadata including prompt context and predict request details
    /// </summary>
    [JsonPropertyName("metadata")]
    public DebugMetadata? Metadata { get; set; }
    
    /// <summary>
    /// Performance metrics for various API operations
    /// </summary>
    [JsonPropertyName("metrics")]
    public DebugMetrics? Metrics { get; set; }
}

/// <summary>
/// Contains metadata information for debugging purposes
/// </summary>
public class DebugMetadata
{
    /// <summary>
    /// Context used for the prompt
    /// </summary>
    [JsonPropertyName("prompt_context")]
    public List<string>? PromptContext { get; set; }
    
    /// <summary>
    /// Details about the predict request
    /// </summary>
    [JsonPropertyName("predict_request")]
    public PredictRequest? PredictRequest { get; set; }
}

/// <summary>
/// Contains details about the predict request
/// </summary>
public class PredictRequest
{
    /// <summary>
    /// The question asked by the user
    /// </summary>
    [JsonPropertyName("question")]
    public string? Question { get; set; }
    
    /// <summary>
    /// The ID of the user making the request
    /// </summary>
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }
    
    /// <summary>
    /// Whether retrieval is enabled
    /// </summary>
    [JsonPropertyName("retrieval")]
    public bool Retrieval { get; set; }
    
    /// <summary>
    /// System prompt if provided
    /// </summary>
    [JsonPropertyName("system")]
    public string? System { get; set; }
    
    /// <summary>
    /// Query context information
    /// </summary>
    [JsonPropertyName("query_context")]
    public Dictionary<string, string>? QueryContext { get; set; }
    
    /// <summary>
    /// Order of query context elements
    /// </summary>
    [JsonPropertyName("query_context_order")]
    public Dictionary<string, int>? QueryContextOrder { get; set; }
    
    /// <summary>
    /// Chat history if any
    /// </summary>
    [JsonPropertyName("chat_history")]
    public List<object>? ChatHistory { get; set; }
    
    /// <summary>
    /// Whether to truncate the response
    /// </summary>
    [JsonPropertyName("truncate")]
    public bool Truncate { get; set; }
    
    /// <summary>
    /// User-provided prompt if any
    /// </summary>
    [JsonPropertyName("user_prompt")]
    public string? UserPrompt { get; set; }
    
    /// <summary>
    /// Whether to include citations
    /// </summary>
    [JsonPropertyName("citations")]
    public bool Citations { get; set; }
    
    /// <summary>
    /// Citation threshold if specified
    /// </summary>
    [JsonPropertyName("citation_threshold")]
    public double? CitationThreshold { get; set; }
    
    /// <summary>
    /// Generative model to use
    /// </summary>
    [JsonPropertyName("generative_model")]
    public string? GenerativeModel { get; set; }
    
    /// <summary>
    /// Maximum tokens to generate
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; set; }
    
    /// <summary>
    /// Query context images if any
    /// </summary>
    [JsonPropertyName("query_context_images")]
    public Dictionary<string, object>? QueryContextImages { get; set; }
    
    /// <summary>
    /// Whether to prefer markdown format
    /// </summary>
    [JsonPropertyName("prefer_markdown")]
    public bool PreferMarkdown { get; set; }
    
    /// <summary>
    /// JSON schema if specified
    /// </summary>
    [JsonPropertyName("json_schema")]
    public string? JsonSchema { get; set; }
    
    /// <summary>
    /// Whether to rerank context
    /// </summary>
    [JsonPropertyName("rerank_context")]
    public bool RerankContext { get; set; }
    
    /// <summary>
    /// Top k results to return
    /// </summary>
    [JsonPropertyName("top_k")]
    public int TopK { get; set; }
    
    /// <summary>
    /// Whether to format the prompt
    /// </summary>
    [JsonPropertyName("format_prompt")]
    public bool FormatPrompt { get; set; }
    
    /// <summary>
    /// Seed for reproducibility
    /// </summary>
    [JsonPropertyName("seed")]
    public int? Seed { get; set; }
}

/// <summary>
/// Contains performance metrics for various API operations
/// </summary>
public class DebugMetrics
{
    /// <summary>
    /// Metrics for the main query operation
    /// </summary>
    [JsonPropertyName("main_query")]
    public MainQueryMetrics? MainQuery { get; set; }
    
    /// <summary>
    /// Metrics for hybrid retrieval
    /// </summary>
    [JsonPropertyName("hybrid_retrieval")]
    public HybridRetrievalMetrics? HybridRetrieval { get; set; }
    
    /// <summary>
    /// Metrics for context building
    /// </summary>
    [JsonPropertyName("context_building")]
    public Dictionary<string, double>? ContextBuilding { get; set; }
    
    /// <summary>
    /// Metrics for the ask operation
    /// </summary>
    [JsonPropertyName("ask")]
    public AskMetrics? Ask { get; set; }
}

/// <summary>
/// Metrics for the main query operation
/// </summary>
public class MainQueryMetrics
{
    /// <summary>
    /// Time taken to parse the query
    /// </summary>
    [JsonPropertyName("query_parse")]
    public double QueryParse { get; set; }
    
    /// <summary>
    /// Time taken to search the index
    /// </summary>
    [JsonPropertyName("index_search")]
    public double IndexSearch { get; set; }
    
    /// <summary>
    /// Time taken to merge results
    /// </summary>
    [JsonPropertyName("results_merge")]
    public double ResultsMerge { get; set; }
}

/// <summary>
/// Metrics for hybrid retrieval
/// </summary>
public class HybridRetrievalMetrics
{
    /// <summary>
    /// Time taken for the main query
    /// </summary>
    [JsonPropertyName("main_query")]
    public double MainQuery { get; set; }
}

/// <summary>
/// Metrics for the ask operation
/// </summary>
public class AskMetrics
{
    /// <summary>
    /// Time taken for retrieval
    /// </summary>
    [JsonPropertyName("retrieval")]
    public double Retrieval { get; set; }
    
    /// <summary>
    /// Time taken for context building
    /// </summary>
    [JsonPropertyName("context_building")]
    public double ContextBuilding { get; set; }
    
    /// <summary>
    /// Time taken to start streaming
    /// </summary>
    [JsonPropertyName("stream_start")]
    public double StreamStart { get; set; }
    
    /// <summary>
    /// Time taken to predict the answer
    /// </summary>
    [JsonPropertyName("stream_predict_answer")]
    public double StreamPredictAnswer { get; set; }
}

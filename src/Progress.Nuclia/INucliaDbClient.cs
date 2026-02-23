using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Progress.Nuclia;
using Progress.Nuclia.Model;
using Progress.Nuclia.Services;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Progress.Nuclia;

/// <summary>
/// Main Progress Agentic RAG SDK client for interacting with the Progress Agentic RAG REST API
/// </summary>
public interface INucliaDbClient
{
    /// <summary>
    /// Configuration for the Progress Agentic RAG client
    /// </summary>
    NucliaDbConfig Config { get; }

    /// <summary>
    /// Knowledge Box operations
    /// </summary>
    IKnowledgeBoxService KnowledgeBoxes { get; }
    
    /// <summary>
    /// Search operations
    /// </summary>

    ISearchService Search { get; }
    
    /// <summary>
    /// Resource operations
    /// </summary>
    IResourceService Resources { get; }
}
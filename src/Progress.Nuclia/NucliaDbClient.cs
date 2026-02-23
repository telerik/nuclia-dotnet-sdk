using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Progress.Nuclia;
using Progress.Nuclia.Model;
using Progress.Nuclia.Services;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Progress.Nuclia;

/// <inheritdoc />
public class NucliaDbClient : INucliaDbClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly bool _disposeHttpClient;
    private readonly ILoggerFactory _loggerFactory;

    /// <inheritdoc />
    public NucliaDbConfig Config { get; }

    /// <inheritdoc />
    public IKnowledgeBoxService KnowledgeBoxes { get; }

    /// <inheritdoc />
    public ISearchService Search { get; }

    /// <inheritdoc />
    public IResourceService Resources { get; }

    /// <summary>
    /// Resource field operations
    /// </summary>
    public IResourceFieldsService ResourceFields { get; }

    /// <summary>
    /// Creates a new Progress Agentic RAG client with the provided configuration
    /// </summary>
    /// <param name="config">Model configuration</param>
    /// <param name="loggerFactory">Optional logger factory for logging</param>
    public NucliaDbClient(NucliaDbConfig config, ILoggerFactory? loggerFactory = null)
        : this(new HttpClient(), config, true, loggerFactory)
    {
    }

    /// <summary>
    /// Creates a new Progress Agentic RAG client with the provided HttpClient and configuration
    /// </summary>
    /// <param name="httpClient">HTTP client to use</param>
    /// <param name="config">Model configuration</param>
    /// <param name="loggerFactory">Optional logger factory for logging</param>
    public NucliaDbClient(HttpClient httpClient, NucliaDbConfig config, ILoggerFactory? loggerFactory = null)
        : this(httpClient, config, false, loggerFactory)
    {
    }

    private NucliaDbClient(HttpClient httpClient, NucliaDbConfig config, bool disposeHttpClient, ILoggerFactory? loggerFactory = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Config = config ?? throw new ArgumentNullException(nameof(config));
        _disposeHttpClient = disposeHttpClient;
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

        // Configure HTTP client
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("X-NUCLIA-SERVICEACCOUNT", $"Bearer {config.ApiKey}");        
        
        // Configure JSON serialization
        _jsonOptions = GetJsonSerializerOptions();

        // Initialize services
        var baseUrl = $"https://{config.ZoneId}.rag.progress.cloud/api/v1";
        
        var knowledgeBoxLogger = _loggerFactory.CreateLogger<KnowledgeBoxService>();
        KnowledgeBoxes = new KnowledgeBoxService(_httpClient, baseUrl, _jsonOptions, knowledgeBoxLogger);
        
        var searchLogger = _loggerFactory.CreateLogger<SearchService>();
        Search = new SearchService(
            _httpClient, 
            baseUrl, 
            _jsonOptions, 
            config.KnowledgeBoxId,
            searchLogger);
        
        var resourceLogger = _loggerFactory.CreateLogger<ResourceService>();
        Resources = new ResourceService(_httpClient, baseUrl, _jsonOptions, config.KnowledgeBoxId, resourceLogger);
        
        var resourceFieldsLogger = _loggerFactory.CreateLogger<ResourceFieldsService>();
        ResourceFields = new ResourceFieldsService(_httpClient, baseUrl, _jsonOptions, config.KnowledgeBoxId, resourceFieldsLogger);
    }

    private JsonSerializerOptions GetJsonSerializerOptions()
    {
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
        
        AddModelConverters(jsonOptions);

        return jsonOptions;
    }

    private void AddModelConverters(JsonSerializerOptions jsonOptions)
    {
        // Dynamically discover and add all JsonConverter types in the Progress.Nuclia.Model namespace
        var converterTypes = typeof(NucliaDbClient).Assembly
            .GetTypes()
            .Where(t => t.Namespace == "Progress.Nuclia.Model" &&
                        !t.IsAbstract && 
                        !t.IsInterface && 
                        typeof(JsonConverter).IsAssignableFrom(t) &&
                        t.GetConstructor(Type.EmptyTypes) != null);

        foreach (var converterType in converterTypes)
        {
            var converter = (JsonConverter?)Activator.CreateInstance(converterType);
            if (converter != null)
            {
                jsonOptions.Converters.Add(converter);
            }
        }
    }

    /// <summary>
    /// Disposes the HTTP client if it was created by this instance
    /// </summary>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient?.Dispose();
        }
    }
}

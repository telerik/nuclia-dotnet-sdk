namespace Progress.Nuclia;

/// <summary>
/// Configuration settings required to connect to a Nuclia knowledge box.
/// </summary>
public record NucliaDbConfig
{
    /// <summary>
    /// Gets the Nuclia zone identifier (e.g. aws-us-east-2-1).
    /// </summary>
    public string ZoneId { get; init; }

    /// <summary>
    /// Gets the unique identifier of the target knowledge box.
    /// </summary>
    public string KnowledgeBoxId { get; init; }

    /// <summary>
    /// Gets the API key used for authenticating requests.
    /// </summary>
    public string ApiKey { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NucliaDbConfig"/> record with the provided settings.
    /// </summary>
    /// <param name="ZoneId">The Nuclia zone identifier.</param>
    /// <param name="KnowledgeBoxId">The knowledge box identifier.</param>
    /// <param name="ApiKey">The API key for authentication.</param>
    public NucliaDbConfig(string ZoneId, string KnowledgeBoxId, string ApiKey)
    {
        this.ZoneId = ZoneId;
        this.KnowledgeBoxId = KnowledgeBoxId;
        this.ApiKey = ApiKey;
    }
}

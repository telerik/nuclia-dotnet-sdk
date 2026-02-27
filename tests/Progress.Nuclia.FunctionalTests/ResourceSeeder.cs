using System;
using System.Threading.Tasks;
using Progress.Nuclia;
using Progress.Nuclia.Model;

/// <summary>
/// Helper class to seed the Nuclia Knowledge Box with test resources using the SDK.
/// Ensures each resource is uniquely identified for test isolation and cleanup.
/// </summary>
public class ResourceSeeder
{
    private readonly NucliaDbClient _client;
    private readonly string _testPrefix;

    /// <summary>
    /// Initializes the seeder with a NucliaDbClient and a test prefix for resource identification.
    /// </summary>
    /// <param name="client">The NucliaDbClient instance to use for API calls.</param>
    /// <param name="testPrefix">A unique prefix to identify test resources.</param>
    public ResourceSeeder(NucliaDbClient client, string testPrefix)
    {
        _client = client;
        _testPrefix = testPrefix;
    }

    /// <summary>
    /// Creates a new resource in the Knowledge Box and uploads the-rag-cookbook.pdf as a file field.
    /// Returns the created resource's UUID if successful, or null if creation failed.
    /// </summary>
    /// <param name="title">The title of the resource.</param>
    /// <param name="summary">The summary/description of the resource.</param>
    /// <returns>The UUID of the created resource, or null if creation failed.</returns>
    public async Task<string> CreateTestResourceAsync(string title, string summary)
    {
        var payload = new CreateResourcePayload
        {
            Title = $"{_testPrefix}-{title}",
            Summary = summary,
            Icon = "application/pdf"

        };

        // Path to the PDF file to upload (absolute path based on project directory)
        var projectDir = AppContext.BaseDirectory;
        var pdfPath = System.IO.Path.Combine(projectDir, "Resources", "the-rag-cookbook.pdf");
        if (!System.IO.File.Exists(pdfPath))
            throw new InvalidOperationException($"File not found: {pdfPath}");


        var fileBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
        // File expects base64 string for payload
        var fileB64 = Convert.ToBase64String(fileBytes);
        var file = new File
        {
            Filename = "the-rag-cookbook.pdf",
            ContentType = "application/pdf",
            Payload = fileB64
        };
        var fileField = new FileField(file);
        payload.Files = new System.Collections.Generic.Dictionary<string, FileField>
        {
            { "pdf", fileField }
        };

        var response = await _client.Resources.CreateResourceAsync(payload);
        if (response.Success && response.Data != null)
        {
            return response.Data.Uuid;
        }
        // Optionally log or throw on error
        throw new InvalidOperationException($"Resource creation failed: {response.Error}");
    }
}

using System;
using System.Threading.Tasks;
using Progress.Nuclia;
using Progress.Nuclia.Model;
using Xunit;

/// <summary>
/// Functional test to validate that CreateResourceAsync can create a resource with a PDF file upload.
/// This test ensures the Nuclia SDK can upload a file and receive a valid resource UUID in response.
/// </summary>
public class CreateResourceFunctionalTest
{
    private readonly NucliaDbClient _client;
    private readonly string _testPrefix;

    public CreateResourceFunctionalTest()
    {
        var zoneId = Environment.GetEnvironmentVariable("NUCLIA_ZONE_ID");
        var kbId = Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        var apiKey = Environment.GetEnvironmentVariable("NUCLIA_API_KEY");
        if (string.IsNullOrEmpty(zoneId) || string.IsNullOrEmpty(kbId) || string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("Missing Nuclia API credentials. Set NUCLIA_ZONE_ID, NUCLIA_KB_ID, NUCLIA_API_KEY.");
        var config = new NucliaDbConfig(zoneId, kbId, apiKey);
        _client = new NucliaDbClient(config);
        _testPrefix = "test-" + Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Validates that CreateResourceAsync can create a resource with a PDF file upload.
    /// The test passes if a valid UUID is returned and no exception is thrown.
    /// </summary>
    [Fact(DisplayName = "Can create resource with PDF file using CreateResourceAsync")]
    [Trait("Category", "Functional")]
    public async Task CanCreateResourceWithPdfFile()
    {
        var payload = new CreateResourcePayload
        {
            Title = $"{_testPrefix}-pdf-upload",
            Summary = "Test resource with PDF upload",
            Icon = "application/pdf"
        };
        var projectDir = AppContext.BaseDirectory;
        var pdfPath = System.IO.Path.Combine(projectDir, "Resources", "the-rag-cookbook.pdf");
        Assert.True(System.IO.File.Exists(pdfPath), $"File not found: {pdfPath}");
        var fileBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
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
        Assert.True(response.Success, $"Resource creation failed: {response.Error}");
        Assert.NotNull(response.Data);
        Assert.False(string.IsNullOrEmpty(response.Data.Uuid), "Returned UUID is null or empty");
    }
}

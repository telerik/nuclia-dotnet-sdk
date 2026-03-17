using System;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to verify that the Nuclia SDK can download a field file by resource ID and field ID.
/// This test ensures that the SDK can retrieve binary file content from a resource field.
/// Expected result: The API call succeeds and returns binary data.
/// </summary>
public class DownloadFieldFileByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can download a field file by resource ID and field ID.
    /// The test passes if the API call succeeds and returns binary content.
    /// </summary>
    [Fact(DisplayName = "Can download field file by ID using DownloadFieldFileAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanDownloadFieldFileById()
    {
        // Step 1: Create a resource with a file field
        var payload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-download-test",
            Summary = "Test resource for file download",
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
            { "testfile", fileField }
        };
        
        var createResponse = await Client.Resources.CreateResourceAsync(payload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Download the field file
        var downloadResponse = await Client.Resources.DownloadFieldFileAsync(resourceId, "testfile");
        
        // Step 3: Verify the download succeeded and contains data
        Assert.NotNull(downloadResponse);
        Assert.True(downloadResponse.Success, $"File download failed: {downloadResponse.Error}");
        Assert.NotNull(downloadResponse.Data);
        Assert.True(downloadResponse.Data.Length > 0, "Downloaded file should have content");
    }
}

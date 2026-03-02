using System;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to validate that CreateResourceAsync can create a resource with a PDF file upload.
/// This test ensures the Nuclia SDK can upload a file and receive a valid resource UUID in response.
/// </summary>
public class CreateResourceFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Validates that CreateResourceAsync can create a resource with a PDF file upload.
    /// The test passes if a valid UUID is returned and no exception is thrown.
    /// </summary>
    [Fact(DisplayName = "Can create resource with PDF file using CreateResourceAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanCreateResourceWithPdfFile()
    {
        var payload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-pdf-upload",
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
        var response = await Client.Resources.CreateResourceAsync(payload);
        Assert.True(response.Success, $"Resource creation failed: {response.Error}");
        Assert.NotNull(response.Data);
        Assert.False(string.IsNullOrEmpty(response.Data.Uuid), "Returned UUID is null or empty");
        
        // Track for cleanup
        CreatedResourceIds.Add(response.Data.Uuid);
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.KnowledgeBox;

/// <summary>
/// Functional test to verify that the Nuclia SDK can upload files to a Knowledge Box.
/// This test ensures that the SDK can perform simplified single file uploads.
/// Expected result: The API call succeeds and file is uploaded.
/// </summary>
public class UploadKnowledgeBoxFileAsyncFunctionalTest : ReadOnlyFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can upload a file to the Knowledge Box.
    /// The test passes if the API call succeeds and returns upload information.
    /// </summary>
    [Fact(DisplayName = "Can upload file to Knowledge Box using UploadKnowledgeBoxFileAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "KnowledgeBoxService")]
    public async Task CanUploadFileToKnowledgeBox()
    {
        // Arrange: Get the KB ID and prepare a test file
        var kbId = System.Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        Assert.False(string.IsNullOrEmpty(kbId), "NUCLIA_KB_ID must be set");

        var projectDir = AppContext.BaseDirectory;
        var pdfPath = Path.Combine(projectDir, "Resources", "the-rag-cookbook.pdf");
        Assert.True(File.Exists(pdfPath), $"File not found: {pdfPath}");

        // Act: Upload the file
        using var fileStream = File.OpenRead(pdfPath);
        var response = await Client.KnowledgeBoxes.UploadKnowledgeBoxFileAsync(
            kbId, 
            fileStream, 
            "the-rag-cookbook.pdf"
        );

        // Assert: Verify the upload response
        Assert.NotNull(response);
        Assert.True(response.Success, $"File upload failed: {response.Error}");
        Assert.NotNull(response.Data);
    }

    /// <summary>
    /// Verifies that the SDK can upload a file with split strategy.
    /// The test passes if the API call succeeds with the split strategy parameter.
    /// </summary>
    [Fact(DisplayName = "Can upload file with split strategy to Knowledge Box")]
    [Trait("Category", "Functional")]
    [Trait("Service", "KnowledgeBoxService")]
    public async Task CanUploadFileWithSplitStrategy()
    {
        // Arrange: Get the KB ID and prepare a test file
        var kbId = System.Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        Assert.False(string.IsNullOrEmpty(kbId), "NUCLIA_KB_ID must be set");

        var projectDir = AppContext.BaseDirectory;
        var pdfPath = Path.Combine(projectDir, "Resources", "the-rag-cookbook.pdf");
        Assert.True(File.Exists(pdfPath), $"File not found: {pdfPath}");

        // Act: Upload the file with split strategy
        using var fileStream = File.OpenRead(pdfPath);
        var response = await Client.KnowledgeBoxes.UploadKnowledgeBoxFileAsync(
            kbId, 
            fileStream, 
            "the-rag-cookbook.pdf",
            splitStrategy: "auto"
        );

        // Assert: Verify the upload response
        Assert.NotNull(response);
        Assert.True(response.Success, $"File upload with split strategy failed: {response.Error}");
        Assert.NotNull(response.Data);
    }
}

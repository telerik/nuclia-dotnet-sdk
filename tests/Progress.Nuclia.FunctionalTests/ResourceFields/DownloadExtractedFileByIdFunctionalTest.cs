using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class DownloadExtractedFileByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can download extracted file by ID using DownloadExtractedFileByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanDownloadExtractedFileById()
    {
        var response = await Client.ResourceFields.DownloadExtractedFileByIdAsync("test-id", FieldTypeName.File, "field", "downloadField");
        Assert.NotNull(response);
    }
}

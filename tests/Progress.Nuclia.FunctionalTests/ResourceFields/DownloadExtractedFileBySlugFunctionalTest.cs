using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class DownloadExtractedFileBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can download extracted file by slug using DownloadExtractedFileBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanDownloadExtractedFileBySlug()
    {
        var response = await Client.ResourceFields.DownloadExtractedFileBySlugAsync("test-slug", FieldTypeName.File, "field", "downloadField");
        Assert.NotNull(response);
    }
}

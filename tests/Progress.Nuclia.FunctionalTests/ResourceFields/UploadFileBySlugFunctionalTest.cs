using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class UploadFileBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can upload file by slug using UploadFileBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanUploadFileBySlug()
    {
        var slug = $"{TestPrefix}-upload";
        var createPayload = new CreateResourcePayload { Title = slug, Slug = slug };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var bytes = System.Text.Encoding.UTF8.GetBytes("test content");
        var response = await Client.ResourceFields.UploadFileBySlugAsync(slug, "uploadfield", bytes, "test.txt");
        Assert.True(response.Success);
    }
}

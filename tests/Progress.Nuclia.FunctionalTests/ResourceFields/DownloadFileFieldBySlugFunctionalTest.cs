using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class DownloadFileFieldBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can download file field by slug using DownloadFileFieldBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanDownloadFileFieldBySlug()
    {
        var slug = $"{TestPrefix}-dl";
        var createPayload = new CreateResourcePayload { Title = slug, Slug = slug };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var file = new File { Filename = "test.txt", ContentType = "text/plain", Payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test")) };
        await Client.ResourceFields.AddFileFieldBySlugAsync(slug, "dlfile", new FileField(file));
        var response = await Client.ResourceFields.DownloadFileFieldBySlugAsync(slug, "dlfile");
        Assert.True(response.Success);
    }
}

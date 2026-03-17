using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class DownloadFileFieldByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can download file field by ID using DownloadFileFieldByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanDownloadFileFieldById()
    {
        var createPayload = new CreateResourcePayload { Title = "$TestPrefix-dl" };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var file = new File { Filename = "test.txt", ContentType = "text/plain", Payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test")) };
        await Client.ResourceFields.AddFileFieldByIdAsync(createResponse.Data.Uuid, "dlfile", new FileField(file));
        var response = await Client.ResourceFields.DownloadFileFieldByIdAsync(createResponse.Data.Uuid, "dlfile");
        Assert.True(response.Success);
    }
}

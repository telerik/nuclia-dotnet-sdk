using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class AddFileFieldByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can add file field by ID using AddFileFieldByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanAddFileFieldById()
    {
        var createPayload = new CreateResourcePayload { Title = "$TestPrefix-file" };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var file = new File { Filename = "test.txt", ContentType = "text/plain", Payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test")) };
        var response = await Client.ResourceFields.AddFileFieldByIdAsync(createResponse.Data.Uuid, "testfile", new FileField(file));
        Assert.True(response.Success);
    }
}

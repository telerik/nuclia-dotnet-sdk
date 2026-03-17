using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class UploadFileByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can upload file by ID using UploadFileByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanUploadFileById()
    {
        var createPayload = new CreateResourcePayload { Title = "$TestPrefix-upload" };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var bytes = System.Text.Encoding.UTF8.GetBytes("test content");
        var response = await Client.ResourceFields.UploadFileByIdAsync(createResponse.Data.Uuid, "uploadfield", bytes, "test.txt");
        Assert.True(response.Success);
    }
}

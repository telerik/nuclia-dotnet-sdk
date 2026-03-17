using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class GetResourceFieldByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can get resource field by ID using GetResourceFieldByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanGetResourceFieldById()
    {
        var createPayload = new CreateResourcePayload { Title = "$TestPrefix-get" };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var textField = new TextField("Test") { Format = TextFormat.PLAIN };
        await Client.ResourceFields.AddTextFieldByIdAsync(createResponse.Data.Uuid, "getfield", textField);
        var response = await Client.ResourceFields.GetResourceFieldByIdAsync(createResponse.Data.Uuid, FieldTypeName.Text, "getfield");
        Assert.True(response.Success);
    }
}

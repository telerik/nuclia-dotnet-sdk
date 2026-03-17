using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class DeleteResourceFieldByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can delete resource field by ID using DeleteResourceFieldByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanDeleteResourceFieldById()
    {
        var createPayload = new CreateResourcePayload { Title = "$TestPrefix-del" };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var textField = new TextField("Test") { Format = TextFormat.PLAIN };
        await Client.ResourceFields.AddTextFieldByIdAsync(createResponse.Data.Uuid, "delfield", textField);
        var response = await Client.ResourceFields.DeleteResourceFieldByIdAsync(createResponse.Data.Uuid, FieldTypeName.Text, "delfield");
        Assert.True(response.Success);
    }
}

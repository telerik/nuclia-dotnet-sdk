using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class AddTextFieldByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can add text field by ID using AddTextFieldByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanAddTextFieldById()
    {
        var createPayload = new CreateResourcePayload { Title = "$TestPrefix-text" };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var textField = new TextField("Test text content") { Format = TextFormat.PLAIN };
        var response = await Client.ResourceFields.AddTextFieldByIdAsync(createResponse.Data.Uuid, "testtext", textField);
        Assert.True(response.Success);
    }
}

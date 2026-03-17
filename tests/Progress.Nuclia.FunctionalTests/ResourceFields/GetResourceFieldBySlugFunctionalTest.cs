using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class GetResourceFieldBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can get resource field by slug using GetResourceFieldBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanGetResourceFieldBySlug()
    {
        var slug = $"{TestPrefix}-get";
        var createPayload = new CreateResourcePayload { Title = slug, Slug = slug };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);  
        var textField = new TextField("Test") { Format = TextFormat.PLAIN };
        await Client.ResourceFields.AddTextFieldBySlugAsync(slug, "getfield", textField);
        var response = await Client.ResourceFields.GetResourceFieldBySlugAsync(slug, FieldTypeName.Text, "getfield");
        Assert.True(response.Success);
    }
}

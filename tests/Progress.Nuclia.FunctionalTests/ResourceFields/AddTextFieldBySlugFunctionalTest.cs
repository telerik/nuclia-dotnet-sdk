using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class AddTextFieldBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can add text field by slug using AddTextFieldBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanAddTextFieldBySlug()
    {
        var slug = $"{TestPrefix}-text";
        var createPayload = new CreateResourcePayload { Title = slug, Slug = slug };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var textField = new TextField("Test text content") { Format = TextFormat.PLAIN };
        var response = await Client.ResourceFields.AddTextFieldBySlugAsync(slug, "testtext", textField);
        Assert.True(response.Success);
    }
}

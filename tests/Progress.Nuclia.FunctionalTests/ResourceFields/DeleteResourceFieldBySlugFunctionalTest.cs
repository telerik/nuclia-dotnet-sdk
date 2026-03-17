using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class DeleteResourceFieldBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can delete resource field by slug using DeleteResourceFieldBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanDeleteResourceFieldBySlug()
    {
        var slug = $"{TestPrefix}-del";
        var createPayload = new CreateResourcePayload { Title = slug, Slug = slug };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var textField = new TextField("Test") { Format = TextFormat.PLAIN };
        await Client.ResourceFields.AddTextFieldBySlugAsync(slug, "delfield", textField);
        var response = await Client.ResourceFields.DeleteResourceFieldBySlugAsync(slug, FieldTypeName.Text, "delfield");
        Assert.True(response.Success);
    }
}

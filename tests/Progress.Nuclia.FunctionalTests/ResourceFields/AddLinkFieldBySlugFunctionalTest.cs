using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class AddLinkFieldBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can add link field by slug using AddLinkFieldBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanAddLinkFieldBySlug()
    {
        var slug = $"{TestPrefix}-link";
        var createPayload = new CreateResourcePayload { Title = slug, Slug = slug };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var linkField = new LinkField("https://example.com");
        var response = await Client.ResourceFields.AddLinkFieldBySlugAsync(slug, "testlink", linkField);
        Assert.True(response.Success);
    }
}

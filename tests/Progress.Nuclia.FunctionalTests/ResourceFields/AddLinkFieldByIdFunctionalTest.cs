using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class AddLinkFieldByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can add link field by ID using AddLinkFieldByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanAddLinkFieldById()
    {
        var createPayload = new CreateResourcePayload { Title = "$TestPrefix-link" };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var linkField = new LinkField("https://example.com");
        var response = await Client.ResourceFields.AddLinkFieldByIdAsync(createResponse.Data.Uuid, "testlink", linkField);
        Assert.True(response.Success);
    }
}

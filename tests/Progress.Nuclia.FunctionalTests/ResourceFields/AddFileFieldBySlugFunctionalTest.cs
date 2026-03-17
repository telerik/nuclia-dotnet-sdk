using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class AddFileFieldBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can add file field by slug using AddFileFieldBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanAddFileFieldBySlug()
    {
        var slug = $"{TestPrefix}-slug";
        var createPayload = new CreateResourcePayload { Title = slug, Slug = slug };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var file = new File { Filename = "test.txt", ContentType = "text/plain", Payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test")) };
        var response = await Client.ResourceFields.AddFileFieldBySlugAsync(slug, "testfile", new FileField(file));
        Assert.True(response.Success);
    }
}


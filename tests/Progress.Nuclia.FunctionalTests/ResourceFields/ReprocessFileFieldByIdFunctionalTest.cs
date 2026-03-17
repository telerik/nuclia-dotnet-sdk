using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class ReprocessFileFieldByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can reprocess file field by ID using ReprocessFileFieldByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanReprocessFileById()
    {
        var createPayload = new CreateResourcePayload { Title = $"{TestPrefix}-reprocess" };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        var file = new File 
        { 
            Filename = "test.txt", 
            ContentType = "text/plain", 
            Payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test")) 
        };
        var addFieldResponse = await Client.ResourceFields.AddFileFieldByIdAsync(resourceId, "rpfile", new FileField(file));
        Assert.True(addFieldResponse.Success, $"Failed to add file field. Error: {addFieldResponse.Error}");

        // Wait for the file to be initially processed before attempting to reprocess
        // The API requires the file to be fully processed before it can be reprocessed
        await Task.Delay(10000);

        var response = await Client.ResourceFields.ReprocessFileFieldByIdAsync(resourceId, "rpfile");
        Assert.True(response.Success, $"Failed to reprocess file field. Error: {response.Error}");
    }
}

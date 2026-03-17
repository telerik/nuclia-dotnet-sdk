using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class AddConversationFieldByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can add conversation field by ID using AddConversationFieldByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanAddConversationFieldById()
    {
        var createPayload = new CreateResourcePayload { Title = "$TestPrefix-conv" };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var content = new InputMessageContent("Test message");
        var message = new InputMessage(content, "msg1");
        var conversationField = new InputConversationField { Messages = new List<InputMessage> { message } };
        var response = await Client.ResourceFields.AddConversationFieldByIdAsync(createResponse.Data.Uuid, "convfield", conversationField);
        Assert.True(response.Success);
    }
}

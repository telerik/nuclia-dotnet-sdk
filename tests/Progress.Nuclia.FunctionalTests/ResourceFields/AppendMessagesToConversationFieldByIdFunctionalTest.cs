using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class AppendMessagesToConversationFieldByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can append messages to conversation field by ID using AppendMessagesToConversationFieldByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanAppendMessagesToConversationFieldById()
    {
        var createResponse = await Client.Resources.CreateResourceAsync(new CreateResourcePayload { Title = "$TestPrefix-append" });
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var content = new InputMessageContent("Test");
        var message = new InputMessage(content, "msg1");
        var field = new InputConversationField { Messages = new List<InputMessage> { message } };
        await Client.ResourceFields.AddConversationFieldByIdAsync(createResponse.Data.Uuid, "convfield", field);
        var newContent = new InputMessageContent("New");
        var newMessage = new InputMessage(newContent, "msg2");
        var messages = new List<InputMessage> { newMessage };
        var response = await Client.ResourceFields.AppendMessagesToConversationFieldByIdAsync(createResponse.Data.Uuid, "convfield", messages);
        Assert.True(response.Success);
    }
}

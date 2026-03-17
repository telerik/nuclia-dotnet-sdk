using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class AppendMessagesToConversationFieldBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can append messages to conversation field by slug using AppendMessagesToConversationFieldBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanAppendMessagesToConversationFieldBySlug()
    {
        var slug = $"{TestPrefix}-append";
        var content = new InputMessageContent("Test");
        var message = new InputMessage(content, "msg1");
        var field = new InputConversationField { Messages = new List<InputMessage> { message } };
        var createResponse = await Client.Resources.CreateResourceAsync(new CreateResourcePayload { Title = slug, Slug = slug });
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        await Client.ResourceFields.AddConversationFieldBySlugAsync(slug, "convfield", field);
        var newContent = new InputMessageContent("New");
        var newMessage = new InputMessage(newContent, "msg2");
        var messages = new List<InputMessage> { newMessage };
        var response = await Client.ResourceFields.AppendMessagesToConversationFieldBySlugAsync(slug, "convfield", messages);
        Assert.True(response.Success);
    }
}


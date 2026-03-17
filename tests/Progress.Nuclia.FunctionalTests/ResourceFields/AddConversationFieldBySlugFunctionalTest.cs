using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class AddConversationFieldBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can add conversation field by slug using AddConversationFieldBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanAddConversationFieldBySlug()
    {
        var slug = $"{TestPrefix}-conv";
        var createPayload = new CreateResourcePayload { Title = slug, Slug = slug };
        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        CreatedResourceIds.Add(createResponse.Data.Uuid);
        var content = new InputMessageContent("Test message");
        var message = new InputMessage(content, "msg1");
        var conversationField = new InputConversationField { Messages = new List<InputMessage> { message } };
        var response = await Client.ResourceFields.AddConversationFieldBySlugAsync(slug, "convfield", conversationField);
        Assert.True(response.Success);
    }
}


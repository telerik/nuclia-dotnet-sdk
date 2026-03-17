using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class DownloadConversationAttachmentByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can download conversation attachment by ID using DownloadConversationAttachmentByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanDownloadConversationAttachmentById()
    {
        var response = await Client.ResourceFields.DownloadConversationAttachmentByIdAsync("test-id", "field", "msg1", 0);
        Assert.NotNull(response);
    }
}

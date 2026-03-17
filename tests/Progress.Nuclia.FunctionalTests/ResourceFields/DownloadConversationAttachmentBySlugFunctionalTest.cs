using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.ResourceFields;

public class DownloadConversationAttachmentBySlugFunctionalTest : WriteOperationFunctionalTestBase
{
    [Fact(DisplayName = "Can download conversation attachment by slug using DownloadConversationAttachmentBySlugAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceFieldsService")]
    public async Task CanDownloadConversationAttachmentBySlug()
    {
        var response = await Client.ResourceFields.DownloadConversationAttachmentBySlugAsync("test-slug", "field", "msg1", 0);
        Assert.NotNull(response);
    }
}

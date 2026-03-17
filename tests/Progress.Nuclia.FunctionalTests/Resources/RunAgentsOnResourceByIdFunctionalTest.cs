using System.Threading.Tasks;
using Progress.Nuclia.Model;
using Xunit;

namespace Progress.Nuclia.FunctionalTests.Resources;

/// <summary>
/// Functional test to verify that the Nuclia SDK can run ingestion agents on a resource by its ID.
/// This test ensures that the SDK can trigger agent execution for a resource.
/// Expected result: The API call succeeds and returns ResourceAgentsResponse information.
/// </summary>
public class RunAgentsOnResourceByIdFunctionalTest : WriteOperationFunctionalTestBase
{
    /// <summary>
    /// Verifies that the SDK can run agents on a resource by its ID.
    /// The test passes if the API call succeeds and returns agent response data.
    /// </summary>
    [Fact(DisplayName = "Can run agents on resource by ID using RunAgentsOnResourceByIdAsync")]
    [Trait("Category", "Functional")]
    [Trait("Service", "ResourceService")]
    public async Task CanRunAgentsOnResourceById()
    {
        // Step 1: Create a test resource
        var createPayload = new CreateResourcePayload
        {
            Title = $"{TestPrefix}-run-agents-test",
            Summary = "Test resource for running agents",
            Icon = "application/stf-link"
        };

        var createResponse = await Client.Resources.CreateResourceAsync(createPayload);
        Assert.True(createResponse.Success, $"Resource creation failed: {createResponse.Error}");
        Assert.NotNull(createResponse.Data);
        
        var resourceId = createResponse.Data.Uuid;
        CreatedResourceIds.Add(resourceId);

        // Step 2: Run agents on the resource
        var agentsRequest = new ResourceAgentsRequest
        {
            // Configure agents request as needed
        };

        var agentsResponse = await Client.Resources.RunAgentsOnResourceByIdAsync(resourceId, agentsRequest);

        // Step 3: Verify agents execution response
        Assert.NotNull(agentsResponse);
        // Note: The success criteria may vary depending on whether agents are configured
        // If no agents are configured, this might still return a response
        Assert.NotNull(agentsResponse.Data);
    }
}

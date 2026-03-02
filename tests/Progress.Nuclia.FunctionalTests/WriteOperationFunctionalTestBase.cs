using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Progress.Nuclia.FunctionalTests;

/// <summary>
/// Base class for functional tests that perform write operations (create, modify, delete).
/// Implements IAsyncLifetime to handle automatic cleanup of created resources.
/// </summary>
public abstract class WriteOperationFunctionalTestBase : FunctionalTestBase, IAsyncLifetime
{
    /// <summary>
    /// List of resource UUIDs created during the test.
    /// These will be automatically cleaned up in DisposeAsync.
    /// </summary>
    protected readonly List<string> CreatedResourceIds = new();

    /// <summary>
    /// Test prefix for creating unique resource names.
    /// </summary>
    protected readonly string TestPrefix;

    protected WriteOperationFunctionalTestBase()
    {
        TestPrefix = "test-" + Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Initialize async - no setup required by default.
    /// Override in derived classes if needed.
    /// </summary>
    public virtual Task InitializeAsync() => Task.CompletedTask;

    /// <summary>
    /// Clean up all created resources after tests complete.
    /// Swallows exceptions to prevent test failures during cleanup.
    /// </summary>
    public virtual async Task DisposeAsync()
    {
        foreach (var resourceId in CreatedResourceIds)
        {
            try
            {
                await Client.Resources.DeleteResourceByIdAsync(resourceId);
            }
            catch
            {
                // Ignore cleanup errors to prevent test failures
            }
        }
    }
}

namespace Progress.Nuclia.FunctionalTests;

/// <summary>
/// Base class for functional tests that only perform read operations.
/// Inherits client initialization from FunctionalTestBase.
/// No cleanup required since no resources are created.
/// </summary>
public abstract class ReadOnlyFunctionalTestBase : FunctionalTestBase
{
}

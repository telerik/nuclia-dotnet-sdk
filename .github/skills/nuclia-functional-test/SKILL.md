---
name: nuclia-functional-test
description: This skill file provides guidelines for creating functional tests for the Progress Nuclia .NET SDK.
---

## Overview

Functional tests verify that the SDK can successfully interact with the real Nuclia API using actual credentials. These tests ensure end-to-end functionality of SDK methods.

## Test Structure Pattern

### 1. File Naming Convention
- Name: `{MethodName}FunctionalTest.cs`
- Example: `ListResourcesFunctionalTest.cs`, `CreateResourceFunctionalTest.cs`

### 2. Class Documentation
Include a comprehensive XML summary that explains:
- What the test verifies
- What API/SDK functionality is being tested
- Expected results

Example:
```csharp
/// <summary>
/// Functional test to verify that the Nuclia SDK can [ACTION] in the configured test Knowledge Box.
/// This test ensures that the SDK is able to connect to the real API using provided credentials and [SPECIFIC OPERATION].
/// Expected result: [EXPECTED OUTCOME].
/// </summary>
```

### 3. Required Using Statements
```csharp
using System;
using System.Threading.Tasks;
using Xunit;
using Progress.Nuclia;

// For tests that create resources requiring cleanup:
using System.Collections.Generic;
```

### 4. Test Class Setup

#### Client Initialization
- Initialize `NucliaDbClient` in the constructor
- Load credentials from environment variables:
  - `NUCLIA_ZONE_ID`
  - `NUCLIA_KB_ID`
  - `NUCLIA_API_KEY`
- Throw `InvalidOperationException` if credentials are missing

```csharp
private readonly NucliaDbClient _client;

public {TestClassName}()
{
    var zoneId = Environment.GetEnvironmentVariable("NUCLIA_ZONE_ID");
    var kbId = Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
    var apiKey = Environment.GetEnvironmentVariable("NUCLIA_API_KEY");

    if (string.IsNullOrEmpty(zoneId) || string.IsNullOrEmpty(kbId) || string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("Missing Nuclia API credentials. Set NUCLIA_ZONE_ID, NUCLIA_KB_ID, NUCLIA_API_KEY.");
    }

    var config = new NucliaDbConfig(zoneId, kbId, apiKey);
    _client = new NucliaDbClient(config);
}
```

### 5. Resource Cleanup (Teardown)

For tests that create resources (POST/PUT operations), implement `IAsyncLifetime` to ensure proper cleanup:

#### When to Use Cleanup
- Tests that create resources (CreateResourceAsync)
- Tests that modify Knowledge Box state
- Any test that leaves persistent data after execution

#### Cleanup Pattern
```csharp
public class {TestClassName} : IAsyncLifetime
{
    private readonly NucliaDbClient _client;
    private readonly List<string> _createdResourceIds = new();

    // Constructor remains the same

    /// <summary>
    /// Initialize async - no setup required.
    /// </summary>
    public Task InitializeAsync() => Task.CompletedTask;

    /// <summary>
    /// Clean up all created resources after tests complete.
    /// </summary>
    public async Task DisposeAsync()
    {
        foreach (var resourceId in _createdResourceIds)
        {
            try
            {
                await _client.Resources.DeleteResourceByIdAsync(resourceId);
            }
            catch
            {
                // Ignore cleanup errors to prevent test failures
            }
        }
    }
}
```

#### Tracking Resources for Cleanup
After creating a resource, add its ID to the tracking list:
```csharp
var createResponse = await _client.Resources.CreateResourceAsync(payload);
var resourceId = createResponse.Data.Uuid;
_createdResourceIds.Add(resourceId);  // Track for cleanup
```

### 6. Test Method Pattern

#### Attributes
- `[Fact(DisplayName = "Descriptive test name")]`
- `[Trait("Category", "Functional")]`

#### Method Signature
- `public async Task {MethodDescription}()`
- Use descriptive names starting with "Can" (e.g., `CanListResources`, `CanCreateResource`)

#### Documentation
```csharp
/// <summary>
/// Verifies that the SDK can [ACTION].
/// The test passes if [SUCCESS CRITERIA].
/// </summary>
```

### 7. Assertions Pattern

For read operations (GET/LIST):
```csharp
var response = await _client.{Service}.{Method}Async();
Assert.NotNull(response);
Assert.True(response.Success, $"API call failed: {response.Error}");
Assert.NotNull(response.Data);
// Add specific data validation as needed
```

For write operations (POST/PUT/DELETE):
```csharp
var response = await _client.{Service}.{Method}Async({parameters});
Assert.NotNull(response);
Assert.True(response.Success, $"API call failed: {response.Error}");
Assert.NotNull(response.Data);
// Verify the operation succeeded with expected data
```

## Best Practices

1. **Keep tests focused**: Each test should verify one specific SDK method or operation
2. **Avoid unnecessary setup**: Only create test data if the test requires it
3. **Clean up test resources**: Implement `IAsyncLifetime` for tests that create resources
4. **Track created resources**: Maintain a list of resource IDs for cleanup in teardown
5. **Use meaningful assertions**: Validate both success and data structure
6. **Provide clear error messages**: Include response.Error in failure messages
7. **Document expected behavior**: Use XML comments to explain what should happen
8. **Use async/await**: All API calls should be asynchronous
9. **Environment variables**: Always load credentials from environment variables, never hardcode
10. **Ignore cleanup errors**: Don't let cleanup failures cause test failures

## Example Templates

### Read-Only Functional Test
```csharp
using System;
using System.Threading.Tasks;
using Xunit;
using Progress.Nuclia;

/// <summary>
/// Functional test to verify that the Nuclia SDK can [describe operation].
/// This test ensures that the SDK is able to connect to the real API using provided credentials and [specific action].
/// Expected result: [expected outcome].
/// </summary>
public class {MethodName}FunctionalTest
{
    private readonly NucliaDbClient _client;

    public {MethodName}FunctionalTest()
    {
        var zoneId = Environment.GetEnvironmentVariable("NUCLIA_ZONE_ID");
        var kbId = Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        var apiKey = Environment.GetEnvironmentVariable("NUCLIA_API_KEY");

        if (string.IsNullOrEmpty(zoneId) || string.IsNullOrEmpty(kbId) || string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Missing Nuclia API credentials. Set NUCLIA_ZONE_ID, NUCLIA_KB_ID, NUCLIA_API_KEY.");
        }

        var config = new NucliaDbConfig(zoneId, kbId, apiKey);
        _client = new NucliaDbClient(config);
    }

    /// <summary>
    /// Verifies that the SDK can [describe what is being verified].
    /// The test passes if [success criteria].
    /// </summary>
    [Fact(DisplayName = "{Descriptive test name}")]
    [Trait("Category", "Functional")]
    public async Task Can{ActionDescription}()
    {
        // Perform operation
        var response = await _client.{Service}.{Method}Async();
        
        // Verify response
        Assert.NotNull(response);
        Assert.True(response.Success, $"API call failed: {response.Error}");
        Assert.NotNull(response.Data);
        // Add specific validations
    }
}
```

### Write Operation Functional Test (with Cleanup)
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Progress.Nuclia;
using Progress.Nuclia.Model;

/// <summary>
/// Functional test to verify that the Nuclia SDK can [create/update/delete operation].
/// This test ensures that the SDK is able to connect to the real API and perform [operation].
/// Expected result: [expected outcome].
/// </summary>
public class {MethodName}FunctionalTest : IAsyncLifetime
{
    private readonly NucliaDbClient _client;
    private readonly List<string> _createdResourceIds = new();

    public {MethodName}FunctionalTest()
    {
        var zoneId = Environment.GetEnvironmentVariable("NUCLIA_ZONE_ID");
        var kbId = Environment.GetEnvironmentVariable("NUCLIA_KB_ID");
        var apiKey = Environment.GetEnvironmentVariable("NUCLIA_API_KEY");

        if (string.IsNullOrEmpty(zoneId) || string.IsNullOrEmpty(kbId) || string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Missing Nuclia API credentials. Set NUCLIA_ZONE_ID, NUCLIA_KB_ID, NUCLIA_API_KEY.");
        }

        var config = new NucliaDbConfig(zoneId, kbId, apiKey);
        _client = new NucliaDbClient(config);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        foreach (var resourceId in _createdResourceIds)
        {
            try
            {
                await _client.Resources.DeleteResourceByIdAsync(resourceId);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    /// <summary>
    /// Verifies that the SDK can [describe operation].
    /// The test passes if [success criteria].
    /// </summary>
    [Fact(DisplayName = "{Descriptive test name}")]
    [Trait("Category", "Functional")]
    public async Task Can{ActionDescription}()
    {
        // Prepare test data
        var payload = new CreateResourcePayload
        {
            Title = "test-resource",
            Summary = "Test resource description"
        };
        
        // Perform operation
        var response = await _client.Resources.CreateResourceAsync(payload);
        
        // Verify response
        Assert.NotNull(response);
        Assert.True(response.Success, $"API call failed: {response.Error}");
        Assert.NotNull(response.Data);
        
        // Track for cleanup
        var resourceId = response.Data.Uuid;
        _createdResourceIds.Add(resourceId);
        
        // Add specific validations
        Assert.False(string.IsNullOrEmpty(resourceId));
    }
}
```

## Running Functional Tests

Functional tests require valid Nuclia API credentials set as environment variables:

```powershell
$env:NUCLIA_ZONE_ID = "your-zone-id"
$env:NUCLIA_KB_ID = "your-kb-id"
$env:NUCLIA_API_KEY = "your-api-key"
```

Run tests:
```powershell
dotnet test --filter "Category=Functional"
```

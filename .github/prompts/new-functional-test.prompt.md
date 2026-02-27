# Create a New Functional Test for IResourceService

## Context
Review the functional testing guidelines in the skill file: `.github/skills/nuclia-functional-test/SKILL.md`

## Task
Create a new functional test for an IResourceService method that is **not yet covered** by existing tests.

## Requirements
1. **Follow the skill guidelines exactly** for file naming, structure, and patterns
2. **File name pattern**: `{MethodName}FunctionalTest.cs` (e.g., `ModifyResourceByIdFunctionalTest.cs`)
3. **Include comprehensive XML documentation** for both the class and test method
4. **Implement IAsyncLifetime** if the test creates resources that need cleanup
5. **Use environment variables** for credentials (NUCLIA_ZONE_ID, NUCLIA_KB_ID, NUCLIA_API_KEY)
6. **Add proper assertions** to verify both Success status and returned Data
7. **Use meaningful test names** starting with "Can" (e.g., `CanModifyResourceById`)
8. **Add test attributes**: `[Fact(DisplayName = "...")]` and `[Trait("Category", "Functional")]`
9. **Create minimal test data** - only what's needed for the specific test
10. **Track created resources** in `_createdResourceIds` list for cleanup if applicable

## Recommended Methods to Test
Consider testing one of these common operations:
- **ModifyResourceByIdAsync** - Tests updating an existing resource (requires creating a resource first)
- **GetResourceBySlugAsync** - Tests retrieving a resource by slug (requires creating a resource with a slug first)
- **ReprocessResourceByIdAsync** - Tests triggering resource reprocessing

## Deliverable
Create a single, complete, well-documented functional test file following all patterns from the skill guidelines.
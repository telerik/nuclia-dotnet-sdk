# Plan: Organize and Expand Functional Tests (Incremental Approach)

The functional test suite will be reorganized into service-specific folders with trait-based grouping and base classes. Instead of implementing all 81 tests at once, work will proceed in phases—one interface at a time—with review checkpoints between each phase.

**Current state:** 6 tests in flat structure, 7.4% coverage (6/81 methods)  
**Approach:** 4 incremental phases with review gates

## Steps

### Phase 0: Foundation (Infrastructure & Refactoring) ✅ COMPLETED

1. **Create base test classes** in [tests/Progress.Nuclia.FunctionalTests/](tests/Progress.Nuclia.FunctionalTests/)
   - `FunctionalTestBase.cs` - Base for all functional tests with `NucliaDbClient` initialization, credential loading from environment variables (`NUCLIA_ZONE_ID`, `NUCLIA_KB_ID`, `NUCLIA_API_KEY`), and validation
   - `ReadOnlyFunctionalTestBase.cs` (inherits `FunctionalTestBase`) - For read operations with no cleanup
   - `WriteOperationFunctionalTestBase.cs` (inherits `FunctionalTestBase`, implements `IAsyncLifetime`) - For write operations with `List<string> _createdResourceIds` tracking and cleanup in `DisposeAsync()`

2. **Create service folder structure**
   - Create [Resources/](tests/Progress.Nuclia.FunctionalTests/Resources/) folder
   - Create [KnowledgeBox/](tests/Progress.Nuclia.FunctionalTests/KnowledgeBox/) folder
   - Create [Search/](tests/Progress.Nuclia.FunctionalTests/Search/) folder
   - Create [ResourceFields/](tests/Progress.Nuclia.FunctionalTests/ResourceFields/) folder

3. **Migrate existing 6 tests** to [Resources/](tests/Progress.Nuclia.FunctionalTests/Resources/)
   - Move files: [ListResourcesFunctionalTest.cs](tests/Progress.Nuclia.FunctionalTests/ListResourcesFunctionalTest.cs), [CreateResourceFunctionalTest.cs](tests/Progress.Nuclia.FunctionalTests/CreateResourceFunctionalTest.cs), [DeleteResourceByIdFunctionalTest.cs](tests/Progress.Nuclia.FunctionalTests/DeleteResourceByIdFunctionalTest.cs), [GetResourceByIdFunctionalTest.cs](tests/Progress.Nuclia.FunctionalTests/GetResourceByIdFunctionalTest.cs), [GetResourceBySlugFunctionalTest.cs](tests/Progress.Nuclia.FunctionalTests/GetResourceBySlugFunctionalTest.cs), [ModifyResourceByIdFunctionalTest.cs](tests/Progress.Nuclia.FunctionalTests/ModifyResourceByIdFunctionalTest.cs)
   - Update namespace to `Progress.Nuclia.FunctionalTests.Resources`
   - Refactor to inherit from `ReadOnlyFunctionalTestBase` or `WriteOperationFunctionalTestBase`
   - Add `[Trait("Service", "ResourceService")]` while keeping `[Trait("Category", "Functional")]`
   - Remove duplicated client initialization code

4. **Move [ResourceSeeder.cs](tests/Progress.Nuclia.FunctionalTests/ResourceSeeder.cs)** to root (stays at test project root as shared utility)

**Verification for Phase 0:**
- Run `dotnet test --filter "Service=ResourceService"` - should find 6 tests, all pass
- Verify base classes compile and tests use them correctly
- Confirm folder structure is in place

---

### Phase 1: Complete IResourceService (19 new tests) ✅ COMPLETED

5. **Implement remaining IResourceService tests** in [Resources/](tests/Progress.Nuclia.FunctionalTests/Resources/) folder
   - `DeleteResourceBySlugFunctionalTest.cs` - Write operation
   - `DownloadFieldFileByIdFunctionalTest.cs` - Read operation, verify `byte[]` content (check length > 0)
   - `DownloadFieldFileByUrlFunctionalTest.cs` - Read operation, verify binary content
   - `ReindexResourceByIdFunctionalTest.cs` - Write operation
   - `ReindexResourceBySlugFunctionalTest.cs` - Write operation
   - `ReprocessResourceByIdFunctionalTest.cs` - Write operation
   - `ReprocessResourceBySlugFunctionalTest.cs` - Write operation
   - `ModifyResourceBySlugFunctionalTest.cs` - Write operation
   - `RunAgentsOnResourceByIdFunctionalTest.cs` - Write operation (if applicable, may be read-only)
   - `SearchWithinResourceByIdFunctionalTest.cs` - Read operation
   - All following skill file patterns: `Can{Action}()` naming, proper traits, cleanup for write ops

**Verification for Phase 1:**
- Run `dotnet test --filter "Service=ResourceService"` - should find 25 tests (6 existing + 19 new), all pass
- Verify 100% coverage of [IResourceService](src/Progress.Nuclia/Services/IResourceService.cs) methods

**🛑 STOP FOR REVIEW** - Validate patterns, structure, test quality before proceeding

---

### Phase 2: Implement ISearchService (18 new tests) ✅ COMPLETED

6. **Implement all ISearchService tests** in [Search/](tests/Progress.Nuclia.FunctionalTests/Search/) folder
   - `AskAsyncFunctionalTest.cs` - Test synchronous Ask (most critical)
   - `AskStreamAsyncFunctionalTest.cs` - Test streaming Ask with `IAsyncEnumerable<>`, iterate and validate chunks
   - `FindAsyncFunctionalTest.cs` - Core search functionality
   - `SearchAsyncFunctionalTest.cs` - Core search functionality  
   - `CatalogAsyncFunctionalTest.cs` - Catalog search
   - `SuggestAsyncFunctionalTest.cs` - Suggestions
   - `SummarizeAsyncFunctionalTest.cs` - Summarization
   - `GraphSearchAsyncFunctionalTest.cs` - Graph search
   - `GraphNodesSearchAsyncFunctionalTest.cs` - Graph nodes
   - `GraphRelationsSearchAsyncFunctionalTest.cs` - Graph relations
   - `SendFeedbackAsyncFunctionalTest.cs` - Feedback submission
   - `AskResourceAsyncFunctionalTest.cs` - Resource-scoped Ask
   - `AskResourceStreamAsyncFunctionalTest.cs` - Resource-scoped streaming Ask
   - `AskResourceBySlugAsyncFunctionalTest.cs` - Resource-scoped Ask by slug
   - `AskResourceBySlugStreamAsyncFunctionalTest.cs` - Resource-scoped streaming Ask by slug
   - `PredictProxyAsyncFunctionalTest.cs` - Predict proxy
   - `ResourceSearchAsyncFunctionalTest.cs` - Resource search
   - All with namespace `Progress.Nuclia.FunctionalTests.Search`
   - All with `[Trait("Service", "SearchService")]`
   - Most are read-only operations (inherit from `ReadOnlyFunctionalTestBase`)

**Verification for Phase 2:**
- Run `dotnet test --filter "Service=SearchService"` - should find 18 tests, all pass
- Verify streaming tests properly handle `IAsyncEnumerable<>` results
- Verify 100% coverage of [ISearchService](src/Progress.Nuclia/Services/ISearchService.cs) methods

**🛑 STOP FOR REVIEW** - Validate search patterns, streaming tests before proceeding

---

### Phase 3: Implement IKnowledgeBoxService (12 new tests)

7. **Implement all IKnowledgeBoxService tests** in [KnowledgeBox/](tests/Progress.Nuclia.FunctionalTests/KnowledgeBox/) folder
   - `GetKnowledgeBoxAsyncFunctionalTest.cs` - Read KB info
   - `GetKnowledgeBoxBySlugAsyncFunctionalTest.cs` - Read KB info by slug
   - `GetKnowledgeBoxCountersAsyncFunctionalTest.cs` - Read counters
   - `GetKnowledgeBoxConfigurationAsyncFunctionalTest.cs` - Read configuration (`Dictionary<string, object>`)
   - `PatchKnowledgeBoxConfigurationAsyncFunctionalTest.cs` - Write operation, partial config update
   - `SetKnowledgeBoxConfigurationAsyncFunctionalTest.cs` - Write operation, full config update
   - `DownloadKnowledgeBoxExportAsyncFunctionalTest.cs` - Export download (may need to trigger export first)
   - `GetKnowledgeBoxExportStatusAsyncFunctionalTest.cs` - Check export status
   - `GetKnowledgeBoxImportStatusAsyncFunctionalTest.cs` - Check import status
   - `UploadKnowledgeBoxFileAsyncFunctionalTest.cs` - Write operation, file upload
   - `GetLearningConfigurationSchemaAsyncFunctionalTest.cs` - Read schema
   - All with namespace `Progress.Nuclia.FunctionalTests.KnowledgeBox`
   - All with `[Trait("Service", "KnowledgeBoxService")]`
   - Configuration write tests need cleanup strategy (restore original config in `DisposeAsync()`)

**Verification for Phase 3:**
- Run `dotnet test --filter "Service=KnowledgeBoxService"` - should find 12 tests, all pass
- Verify configuration modifications are properly cleaned up
- Verify 100% coverage of [IKnowledgeBoxService](src/Progress.Nuclia/Services/IKnowledgeBoxService.cs) methods

**🛑 STOP FOR REVIEW** - Validate KB patterns, config management before proceeding

---

### Phase 4: Implement IResourceFieldsService (26 new tests)

8. **Implement all IResourceFieldsService tests** in [ResourceFields/](tests/Progress.Nuclia.FunctionalTests/ResourceFields/) folder

   **Conversation fields (6 tests):**
   - `AddConversationFieldByIdFunctionalTest.cs`
   - `AddConversationFieldBySlugFunctionalTest.cs`
   - `AppendMessagesToConversationFieldByIdFunctionalTest.cs`
   - `AppendMessagesToConversationFieldBySlugFunctionalTest.cs`
   - `DownloadConversationAttachmentByIdFunctionalTest.cs`
   - `DownloadConversationAttachmentBySlugFunctionalTest.cs`

   **File fields (8 tests):**
   - `AddFileFieldByIdFunctionalTest.cs`
   - `AddFileFieldBySlugFunctionalTest.cs`
   - `UploadFileByIdFunctionalTest.cs`
   - `UploadFileBySlugFunctionalTest.cs`
   - `DownloadFileFieldByIdFunctionalTest.cs`
   - `DownloadFileFieldBySlugFunctionalTest.cs`
   - `ReprocessFileFieldByIdFunctionalTest.cs`
   - `ReprocessFileFieldBySlugFunctionalTest.cs`

   **Link and text fields (4 tests):**
   - `AddLinkFieldByIdFunctionalTest.cs`
   - `AddLinkFieldBySlugFunctionalTest.cs`
   - `AddTextFieldByIdFunctionalTest.cs`
   - `AddTextFieldBySlugFunctionalTest.cs`

   **Generic field operations (8 tests):**
   - `GetResourceFieldByIdFunctionalTest.cs`
   - `GetResourceFieldBySlugFunctionalTest.cs`
   - `DeleteResourceFieldByIdFunctionalTest.cs`
   - `DeleteResourceFieldBySlugFunctionalTest.cs`
   - `DownloadExtractedFileByIdFunctionalTest.cs`
   - `DownloadExtractedFileBySlugFunctionalTest.cs`
   
   - All with namespace `Progress.Nuclia.FunctionalTests.ResourceFields`
   - All with `[Trait("Service", "ResourceFieldsService")]`
   - File upload tests reuse [Resources/the-rag-cookbook.pdf](tests/Progress.Nuclia.FunctionalTests/Resources/the-rag-cookbook.pdf)

9. **Enhance [ResourceSeeder.cs](tests/Progress.Nuclia.FunctionalTests/ResourceSeeder.cs)** if needed during Phase 4
   - Add helper methods for creating resources with specific field types
   - Add methods for generating test messages, conversation data

**Verification for Phase 4:**
- Run `dotnet test --filter "Service=ResourceFieldsService"` - should find 26 tests, all pass
- Verify field operations properly clean up created fields
- Verify 100% coverage of [IResourceFieldsService](src/Progress.Nuclia/Services/IResourceFieldsService.cs) methods

**🛑 STOP FOR REVIEW** - Validate field operation patterns

---

## Final Verification (After all phases complete)

Run comprehensive test suite:
- `dotnet test --filter "Category=Functional"` - All 81 tests pass
- `dotnet test --filter "Service=ResourceService"` - 25 tests
- `dotnet test --filter "Service=SearchService"` - 18 tests
- `dotnet test --filter "Service=KnowledgeBoxService"` - 12 tests
- `dotnet test --filter "Service=ResourceFieldsService"` - 26 tests

Validate final structure:
- Each service's tests isolated in dedicated folder
- All tests follow skill file conventions
- All tests have proper traits for filtering
- Base classes eliminate boilerplate
- 100% method coverage achieved

## Decisions

- **Phased implementation**: 5 phases (Phase 0 foundation + 4 service phases) with review gates to manage complexity
- **Service-first order**: IResourceService → ISearchService → IKnowledgeBoxService → IResourceFieldsService (building from simple CRUD to complex operations)
- **Review checkpoints**: Stop after each phase to validate patterns before proceeding
- **Incremental delivery**: Each phase delivers working, testable functionality
- **Folder structure by service**: Clearer organization, easier navigation in IDE
- **Trait-based grouping**: Flexible filtering without tight coupling to xUnit collections
- **Base classes**: Reduces ~30 lines of boilerplate per test (credential loading, client init, cleanup)
- **Comprehensive coverage**: All 81 methods get tests for maintainability and regression prevention
- **Keep skill file patterns**: Maintain consistency with established `Can{Action}()` naming and cleanup practices

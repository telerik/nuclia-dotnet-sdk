Plan: Real API Functional Tests for SDK

Design and implement functional tests that exercise the nuclia-dotnet-sdk against the real Nuclia API, ensuring safe, isolated, and repeatable testing. The plan addresses environment setup, credential management, test data isolation, resource cleanup, and best practices to avoid production interference.

Steps
1. Create a new test project (e.g., tests/Progress.Nuclia.FunctionalTests/) to separate functional tests from unit tests.
2. Use .NET user secrets or environment variables to securely load API credentials (ApiKey, ZoneId, KnowledgeBoxId).
3. Require a dedicated Nuclia Knowledge Box for testing; document setup and risks.
4. Prefix all test-created resources with a unique identifier (e.g., "test-" + GUID) to ensure isolation and avoid collisions.
5. Implement setup/teardown logic in test fixtures to create and clean up test data, ensuring all resources are deleted after tests.
6. Use ListResourcesAsync as the correct method for listing resources in the Knowledge Box in functional tests.
7. Mark functional tests with traits (e.g., [Trait("Category", "Functional")]) and allow skipping if credentials/environment are missing.
8. Document how to run functional tests, required environment setup, and safety precautions.
9. Ensure tests are idempotent and do not depend on pre-existing data.
10. Each test must be documented with a human-readable description of what is tested, why, and what the expected results are.

Verification
- Run functional tests locally and in CI/CD with test credentials.
- Confirm all test-created resources are cleaned up after execution.
- Validate that tests are skipped if credentials are missing.
- Review documentation for clarity and completeness.

Decisions
- Functional tests use a dedicated test KB and never production data.
- Credentials are managed via secrets/environment, not hardcoded.
- Functional tests are separated from unit tests for clarity and safety.

Ready to refine or expand this plan with sample test templates or specific scenarios if needed.

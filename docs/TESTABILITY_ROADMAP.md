# Testability Improvements Roadmap

This document provides a prioritized roadmap for implementing the testability improvements identified in [TESTABILITY_ANALYSIS.md](./TESTABILITY_ANALYSIS.md).

## ✅ Phase 1: Critical Infrastructure (COMPLETED)

### 1.1 Testing Packages ✅
- [x] Add Testcontainers.PostgreSql 3.9.0
- [x] Add Testcontainers.Redis 3.9.0
- [x] Add Testcontainers.RabbitMq 3.9.0
- [x] Add Moq 4.20.70
- [x] Add WireMock.Net 1.5.62

**Status**: All packages added to `src/Directory.Packages.props`

### 1.2 PostgreSQL Test Fixture ✅
- [x] Create `PostgresTestFixture` base class
- [x] Add to Common.Infrastructure.Tests
- [x] Create example integration tests
- [x] Validate with actual test run

**Status**: Fixture created and validated. All tests passing.

**Files**:
- `src/api/common/TavernTrashers.Api.Common.Infrastructure.Tests/Fixtures/PostgresTestFixture.cs`
- `src/api/common/TavernTrashers.Api.Common.Infrastructure.Tests/Integration/PostgresTestFixtureIntegrationTests.cs`

### 1.3 HTTP Client Abstractions ✅
- [x] Create `IKeyCloakClient` interface
- [x] Update `KeyCloakClient` to implement interface
- [x] Move `UserRepresentation` and `CredentialRepresentation` to Application layer
- [x] Update DI registration
- [x] Validate build and tests

**Status**: Refactoring complete. Build successful, all tests passing.

**Files**:
- `src/api/modules/users/TavernTrashers.Api.Modules.Users.Application/Abstractions/Identity/IKeyCloakClient.cs`
- `src/api/modules/users/TavernTrashers.Api.Modules.Users.Application/Abstractions/Identity/UserRepresentation.cs`
- `src/api/modules/users/TavernTrashers.Api.Modules.Users.Application/Abstractions/Identity/CredentialRepresentation.cs`
- `src/api/modules/users/TavernTrashers.Api.Modules.Users.Infrastructure/Identity/KeyCloakClient.cs`
- `src/api/modules/users/TavernTrashers.Api.Modules.Users.Infrastructure/UsersModule.cs`

### 1.4 Documentation ✅
- [x] Create TESTABILITY_ANALYSIS.md (34KB)
- [x] Create TESTING_GUIDELINES.md (13KB)
- [x] Document all identified issues
- [x] Provide refactoring examples

**Status**: Comprehensive documentation complete.

---

## 🔄 Phase 2: Background Processing (NOT STARTED)

**Estimated Effort**: 2-3 days  
**Priority**: Medium  
**Dependencies**: Phase 1 complete

### 2.1 Extract Message Processing Logic
- [ ] Create `IOutboxMessageProcessor` interface
- [ ] Implement `OutboxMessageProcessor` concrete class
- [ ] Create `IInboxMessageProcessor` interface
- [ ] Implement `InboxMessageProcessor` concrete class
- [ ] Update job classes to use processors

**Benefits**:
- Message processing logic can be unit tested independently
- Database operations remain in jobs (integration tested)
- Easier to test error scenarios
- Better separation of concerns

**Files to Create**:
- `src/api/common/TavernTrashers.Api.Common.Application/Messaging/IOutboxMessageProcessor.cs`
- `src/api/common/TavernTrashers.Api.Common.Infrastructure/Outbox/OutboxMessageProcessor.cs`
- `src/api/common/TavernTrashers.Api.Common.Application/Messaging/IInboxMessageProcessor.cs`
- `src/api/common/TavernTrashers.Api.Common.Infrastructure/Inbox/InboxMessageProcessor.cs`

**Files to Update**:
- `src/api/common/TavernTrashers.Api.Common.Infrastructure/Outbox/ProcessOutboxJobBase.cs`
- `src/api/common/TavernTrashers.Api.Common.Infrastructure/Inbox/ProcessInboxJobBase.cs`

### 2.2 Improve Pipeline Behaviors
- [ ] Create `IModuleContextResolver` interface
- [ ] Implement `ModuleContextResolver`
- [ ] Update `UnitOfWorkSaveChangesPipelineBehavior`
- [ ] Add unit tests for behavior

**Benefits**:
- Service Locator pattern isolated
- Behavior logic can be unit tested with mocks
- Verification of SaveChanges calls is straightforward
- Error cases (missing module) can be tested

**Files to Create**:
- `src/api/common/TavernTrashers.Api.Common.Application/Abstractions/IModuleContextResolver.cs`
- `src/api/common/TavernTrashers.Api.Common.Infrastructure/Modules/ModuleContextResolver.cs`
- `src/api/common/TavernTrashers.Api.Common.Application.Tests/Behaviors/UnitOfWorkSaveChangesPipelineBehaviorTests.cs`

**Files to Update**:
- `src/api/common/TavernTrashers.Api.Common.Application/Behaviors/UnitOfWorkSaveChangesPipelineBehavior.cs`

---

## 🎯 Phase 3: Supporting Infrastructure (NOT STARTED)

**Estimated Effort**: 2-3 days  
**Priority**: Low  
**Dependencies**: Phase 2 complete

### 3.1 Redis Cache Testing Infrastructure
- [ ] Create `RedisTestFixture` class
- [ ] Create `InMemoryDistributedCache` for unit tests
- [ ] Add example cache integration tests
- [ ] Document cache testing patterns

**Benefits**:
- Unit tests don't require Redis
- Integration tests verify actual Redis behavior
- Serialization edge cases can be tested
- Cache expiration logic is testable

**Files to Create**:
- `src/api/common/TavernTrashers.Api.Common.Infrastructure.Tests/Fixtures/RedisTestFixture.cs`
- `src/api/common/TavernTrashers.Api.Common.Infrastructure.Tests/Caching/InMemoryDistributedCache.cs`
- `src/api/common/TavernTrashers.Api.Common.Infrastructure.Tests/Caching/CacheServiceTests.cs`
- `src/api/common/TavernTrashers.Api.Common.Infrastructure.Tests/Integration/CacheServiceIntegrationTests.cs`

### 3.2 RabbitMQ Testing Infrastructure
- [ ] Create `RabbitMqTestFixture` class
- [ ] Add example messaging integration tests
- [ ] Document message bus testing patterns

**Files to Create**:
- `src/api/common/TavernTrashers.Api.Common.Infrastructure.Tests/Fixtures/RabbitMqTestFixture.cs`
- `src/api/common/TavernTrashers.Api.Common.Infrastructure.Tests/Integration/EventBusIntegrationTests.cs`

### 3.3 Test Data Builders
- [ ] Create `RollBuilder` for Dice module
- [ ] Create `CharacterBuilder` for Characters module
- [ ] Create `UserBuilder` for Users module
- [ ] Document builder patterns

**Benefits**:
- Simplifies test setup
- Reduces boilerplate
- Makes tests more readable
- Easier to create valid test data

**Files to Create**:
- `src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Domain.Tests/Builders/RollBuilder.cs`
- `src/api/modules/characters/TavernTrashers.Api.Modules.Characters.Domain.Tests/Builders/CharacterBuilder.cs`
- `src/api/modules/users/TavernTrashers.Api.Modules.Users.Domain.Tests/Builders/UserBuilder.cs`

### 3.4 Repository Integration Tests
- [ ] Add integration tests for `RollRepository`
- [ ] Add integration tests for `CharacterRepository`
- [ ] Add integration tests for `UserRepository`
- [ ] Create `DbContextFactory` helpers

**Files to Create**:
- `src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Infrastructure.Tests/Database/DiceDbContextFactory.cs`
- `src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Infrastructure.Tests/Integration/RollRepositoryIntegrationTests.cs`
- Similar files for other modules

---

## 📝 Phase 4: Documentation and Cleanup (NOT STARTED)

**Estimated Effort**: 1 day  
**Priority**: Low  
**Dependencies**: Phases 1-3 complete

### 4.1 Review and Simplify Abstractions
- [ ] Evaluate `PermissionService` (thin wrapper)
- [ ] Add validation and caching if keeping service
- [ ] OR remove service and use ISender directly
- [ ] Document decision and rationale

### 4.2 Update Documentation
- [ ] Add examples to TESTING_GUIDELINES.md based on new tests
- [ ] Update TESTABILITY_ANALYSIS.md with completion status
- [ ] Create migration guide for existing tests
- [ ] Add troubleshooting section

---

## 📊 Success Metrics

### Coverage Goals
- [ ] Unit test coverage > 80%
- [ ] Integration test coverage for all repositories
- [ ] Integration test coverage for all HTTP clients
- [ ] Integration test coverage for background jobs

### Quality Goals
- [ ] All tests run in < 2 minutes (unit + integration)
- [ ] Zero flaky tests
- [ ] All tests deterministic
- [ ] Clear test failure messages

### Maintainability Goals
- [ ] Consistent test patterns across modules
- [ ] Reusable test fixtures and builders
- [ ] Well-documented testing practices
- [ ] Easy to add new tests

---

## 🎓 Learning Resources

### Testcontainers
- [Testcontainers for .NET Documentation](https://dotnet.testcontainers.org/)
- [PostgreSQL Module](https://dotnet.testcontainers.org/modules/postgres/)
- [Redis Module](https://dotnet.testcontainers.org/modules/redis/)

### Testing Best Practices
- [xUnit Best Practices](https://xunit.net/docs/getting-started/netcore/cmdline)
- [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [Martin Fowler - Testing Strategies](https://martinfowler.com/testing/)

### Mocking
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [WireMock.Net Documentation](https://github.com/WireMock-Net/WireMock.Net/wiki)

---

## 🚀 Getting Started

To start working on Phase 2 or 3:

1. Review the detailed specifications in [TESTABILITY_ANALYSIS.md](./TESTABILITY_ANALYSIS.md)
2. Read the patterns in [TESTING_GUIDELINES.md](./TESTING_GUIDELINES.md)
3. Look at existing test examples in:
   - `src/api/common/TavernTrashers.Api.Common.Infrastructure.Tests/Integration/`
   - `src/api/common/TavernTrashers.Api.Common.Domain.Tests/`
   - `src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Domain.Tests/`
4. Follow the file creation checklist for your chosen phase
5. Run tests frequently: `dotnet test src/TavernTrashers.slnx`

---

## 📌 Notes

- **Test Execution Time**: Integration tests using Testcontainers take ~8 seconds to initialize. Use `IClassFixture` to share containers across tests.
- **Docker Required**: Testcontainers requires Docker to be running. CI/CD pipelines must have Docker available.
- **Parallel Execution**: xUnit runs test classes in parallel by default. Use `[Collection]` attribute if tests need to run sequentially.
- **Cleanup**: Always use `IAsyncLifetime` for async fixture setup/teardown to ensure proper resource disposal.

---

## 🔗 Related Documents

- [TESTABILITY_ANALYSIS.md](./TESTABILITY_ANALYSIS.md) - Detailed analysis and refactoring proposals
- [TESTING_GUIDELINES.md](./TESTING_GUIDELINES.md) - Testing best practices and patterns
- [ARCHITECTURE.md](./ARCHITECTURE.md) - Overall architecture documentation
- [DEVELOPMENT.md](./DEVELOPMENT.md) - Development environment setup

---

**Last Updated**: 2026-02-17  
**Status**: Phase 1 Complete ✅  
**Next Phase**: Phase 2 - Background Processing

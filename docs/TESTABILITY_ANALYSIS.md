# Testability Analysis & Refactoring Recommendations

## Executive Summary

This document analyzes the Tavern Trashers backend codebase to identify components that are difficult to test and proposes refactorings to improve testability for both unit tests and test-container integration tests.

### Key Findings

1. **Missing Test Infrastructure**: No integration test support (Testcontainers, database fixtures)
2. **Hard Dependencies**: Direct coupling to HttpClient, DbContext, and external services
3. **Static Methods**: Difficult-to-mock static factory methods and utilities
4. **Concrete Dependencies**: Several services depend on concrete types instead of abstractions
5. **Side Effects**: Background jobs and messaging infrastructure have observable side effects

---

## Identified Issues & Proposed Refactorings

### 1. HttpClient-Based Services (HIGH PRIORITY)

#### **Issue: KeyCloakClient & KeyCloakTokenClient**

**Location**: `src/api/modules/users/TavernTrashers.Api.Modules.Users.Infrastructure/Identity/`

**Problem**:
```csharp
internal sealed class KeyCloakClient(HttpClient httpClient)
{
    internal async Task<string> RegisterUserAsync(UserRepresentation user, CancellationToken cancellationToken = default)
    {
        var httpResponseMessage = await httpClient.PostAsJsonAsync("users", user, cancellationToken);
        httpResponseMessage.EnsureSuccessStatusCode();
        return ExtractIdentityIdFromLocationHeader(httpResponseMessage);
    }
}
```

**Testability Issues**:
- Direct HttpClient dependency makes unit testing difficult
- Requires actual HTTP responses or complex mocking
- Static method `ExtractIdentityIdFromLocationHeader` has string parsing logic that should be tested separately
- Exception handling via HTTP status codes is not easily testable

**Proposed Refactor**:

**Option A: Extract Interface (Recommended)**
```csharp
// Create abstraction
public interface IKeyCloakClient
{
    Task<string> RegisterUserAsync(UserRepresentation user, CancellationToken cancellationToken = default);
}

// Implementation remains the same but implements interface
internal sealed class KeyCloakClient(HttpClient httpClient) : IKeyCloakClient
{
    // ... existing implementation
}

// Usage in KeyCloakIdentityProviderService
internal sealed class KeyCloakIdentityProviderService(
    IKeyCloakClient keyCloakClient,  // <- Now using interface
    IKeyCloakTokenClient keyCloakTokenClient,
    ILogger<KeyCloakIdentityProviderService> logger)
    : IIdentityProviderService
{
    // ... rest of code unchanged
}
```

**Option B: Extract Header Parser**
```csharp
// New testable class
internal static class LocationHeaderParser
{
    public static string ExtractUserId(string? locationHeader)
    {
        const string usersSegmentName = "users/";
        
        if (locationHeader is null)
            throw new ArgumentNullException(nameof(locationHeader));

        var userSegmentValueIndex = locationHeader.IndexOf(
            usersSegmentName, 
            StringComparison.InvariantCultureIgnoreCase);
        
        if (userSegmentValueIndex == -1)
            throw new InvalidOperationException("Location header does not contain users segment");

        return locationHeader[(userSegmentValueIndex + usersSegmentName.Length)..];
    }
}

// Unit test example
public class LocationHeaderParserTests
{
    [Theory]
    [InlineData("https://auth.example.com/admin/realms/master/users/123-456-789", "123-456-789")]
    [InlineData("/users/abc-def", "abc-def")]
    public void ExtractUserId_ValidHeader_ReturnsUserId(string locationHeader, string expected)
    {
        var result = LocationHeaderParser.ExtractUserId(locationHeader);
        result.ShouldBe(expected);
    }
}
```

**Integration Test Support**:
```csharp
// Create test fixture using WireMock or similar
public class KeyCloakClientIntegrationTests : IAsyncLifetime
{
    private WireMockServer _mockServer;
    private KeyCloakClient _client;

    public async Task InitializeAsync()
    {
        _mockServer = WireMockServer.Start();
        var httpClient = new HttpClient { BaseAddress = new Uri(_mockServer.Urls[0]) };
        _client = new KeyCloakClient(httpClient);
    }

    [Fact]
    public async Task RegisterUserAsync_SuccessfulRegistration_ReturnsUserId()
    {
        // Arrange
        _mockServer
            .Given(Request.Create().WithPath("/users").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithHeader("Location", "/users/test-user-id"));

        // Act
        var result = await _client.RegisterUserAsync(new UserRepresentation(...));

        // Assert
        result.ShouldBe("test-user-id");
    }
}
```

**Benefits**:
- ✅ Unit tests can mock the interface without real HTTP calls
- ✅ Integration tests can use WireMock or Testcontainers for Keycloak
- ✅ Header parsing logic can be tested independently
- ✅ Easier to test error scenarios

---

### 2. Database Repositories (HIGH PRIORITY)

#### **Issue: Direct DbContext Dependencies**

**Location**: All repository implementations
- `src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Infrastructure/Dice/RollRepository.cs`
- `src/api/modules/users/TavernTrashers.Api.Modules.Users.Infrastructure/Users/UserRepository.cs`
- `src/api/modules/characters/TavernTrashers.Api.Modules.Characters.Infrastructure/Characters/CharacterRepository.cs`

**Problem**:
```csharp
internal sealed class RollRepository(DiceDbContext dbContext) : IRollRepository
{
    public async Task<Result<Roll>> GetAsync(Guid rollId, CancellationToken cancellationToken = default) =>
        await dbContext
            .Rolls
            .Include(roll => roll.Parent)
            .Include(roll => roll.Children)
            .SingleOrDefaultAsync(roll => roll.Id == rollId, cancellationToken)
            .ToResultAsync(RollErrors.NotFound(rollId));
}
```

**Testability Issues**:
- Requires actual database for testing (no in-memory option since using PostgreSQL-specific features)
- EF Core's DbContext is concrete, making pure unit testing difficult
- Include chains are not easily verifiable without executing queries
- No test data builders or seeding helpers

**Proposed Refactor**:

**Option A: Testcontainers Integration Tests (Recommended)**

Create shared test infrastructure for database testing:

```csharp
// New file: src/api/common/TavernTrashers.Api.Common.Infrastructure.Tests/Database/PostgresTestFixture.cs
public sealed class PostgresTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("tavern_trashers_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        ConnectionString = _postgres.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        await _postgres.StopAsync();
        await _postgres.DisposeAsync();
    }
}

// New file: src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Infrastructure.Tests/Database/DiceDbContextFactory.cs
public class DiceDbContextFactory
{
    public static DiceDbContext Create(string connectionString)
    {
        var options = new DbContextOptionsBuilder<DiceDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        var context = new DiceDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}

// Integration test example
public class RollRepositoryIntegrationTests : IClassFixture<PostgresTestFixture>
{
    private readonly PostgresTestFixture _fixture;

    public RollRepositoryIntegrationTests(PostgresTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAsync_ExistingRoll_ReturnsRoll()
    {
        // Arrange
        await using var context = DiceDbContextFactory.Create(_fixture.ConnectionString);
        var repository = new RollRepository(context);
        
        var roll = Roll.Create(...);
        context.Rolls.Add(roll);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAsync(roll.Id);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(roll.Id);
    }
}
```

**Option B: Test Data Builders**

Create builders for complex domain entities:

```csharp
// New file: src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Domain.Tests/Builders/RollBuilder.cs
public class RollBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _expression = "1d20";
    private int _value = 10;
    private Roll? _parent;

    public RollBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public RollBuilder WithExpression(string expression)
    {
        _expression = expression;
        return this;
    }

    public RollBuilder WithParent(Roll parent)
    {
        _parent = parent;
        return this;
    }

    public Roll Build()
    {
        // Use reflection or factory method to create Roll
        return Roll.Create(_expression, _value, ...);
    }
}

// Usage in tests
[Fact]
public async Task GetAsync_RollWithParent_IncludesParent()
{
    // Arrange
    var parent = new RollBuilder().WithExpression("1d20").Build();
    var child = new RollBuilder().WithParent(parent).Build();
    
    await using var context = DiceDbContextFactory.Create(_fixture.ConnectionString);
    context.Rolls.AddRange(parent, child);
    await context.SaveChangesAsync();

    var repository = new RollRepository(context);

    // Act
    var result = await repository.GetAsync(child.Id);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    result.Value.Parent.ShouldNotBeNull();
    result.Value.Parent.Id.ShouldBe(parent.Id);
}
```

**Benefits**:
- ✅ Integration tests verify actual database behavior
- ✅ Tests PostgreSQL-specific features (JSONB, etc.)
- ✅ Test data builders simplify test setup
- ✅ Shared fixtures reduce test execution time
- ✅ More confidence in repository implementations

---

### 3. Background Jobs (MEDIUM PRIORITY)

#### **Issue: Outbox & Inbox Processing Jobs**

**Location**: 
- `src/api/common/TavernTrashers.Api.Common.Infrastructure/Outbox/ProcessOutboxJobBase.cs`
- `src/api/common/TavernTrashers.Api.Common.Infrastructure/Inbox/ProcessInboxJobBase.cs`

**Problem**:
```csharp
public abstract class ProcessOutboxJobBase(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    ILogger logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await using var connection = await dbConnectionFactory.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        
        var outboxMessages = await GetUnprocessedOutboxMessagesAsync(connection, transaction);
        
        foreach (var outboxMessage in outboxMessages)
        {
            var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, ...)!;
            await PublishDomainEvent(domainEvent);
            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }
        
        await transaction.CommitAsync();
    }
}
```

**Testability Issues**:
- Complex orchestration of database operations, deserialization, and event publishing
- Multiple side effects (database updates, event publishing, logging)
- Uses Dapper for raw SQL, making it difficult to verify without actual database
- Service locator pattern (`DomainEventHandlersFactory.GetHandlers`) is hard to test
- Exception handling needs testing across multiple scenarios

**Proposed Refactor**:

**Option A: Extract Strategy Pattern**

```csharp
// New abstraction for outbox message processing
public interface IOutboxMessageProcessor
{
    Task<ProcessingResult> ProcessMessageAsync(
        OutboxMessage message, 
        CancellationToken cancellationToken = default);
}

public sealed record ProcessingResult(bool Success, Exception? Error);

// Concrete implementation
internal sealed class OutboxMessageProcessor(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<OutboxMessageProcessor> logger) : IOutboxMessageProcessor
{
    public async Task<ProcessingResult> ProcessMessageAsync(
        OutboxMessage message, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(message.Content, SerializerSettings.Instance)!;
            
            using var scope = serviceScopeFactory.CreateScope();
            var handlers = DomainEventHandlersFactory.GetHandlers(
                domainEvent.GetType(),
                scope.ServiceProvider,
                message.ModuleAssembly);

            foreach (var handler in handlers)
            {
                await handler.Handle(domainEvent, cancellationToken);
            }

            return new ProcessingResult(true, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
            return new ProcessingResult(false, ex);
        }
    }
}

// Simplified job
public abstract class ProcessOutboxJobBase(
    IDbConnectionFactory dbConnectionFactory,
    IOutboxMessageProcessor messageProcessor,
    IDateTimeProvider dateTimeProvider,
    ILogger logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await using var connection = await dbConnectionFactory.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        
        var outboxMessages = await GetUnprocessedOutboxMessagesAsync(connection, transaction);
        
        foreach (var outboxMessage in outboxMessages)
        {
            var result = await messageProcessor.ProcessMessageAsync(outboxMessage);
            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, result.Error);
        }
        
        await transaction.CommitAsync();
    }
}
```

**Unit Test Example**:
```csharp
public class OutboxMessageProcessorTests
{
    [Fact]
    public async Task ProcessMessageAsync_ValidDomainEvent_ReturnsSuccess()
    {
        // Arrange
        var serviceScopeFactory = CreateMockServiceScopeFactory();
        var logger = new NullLogger<OutboxMessageProcessor>();
        var processor = new OutboxMessageProcessor(serviceScopeFactory, logger);
        
        var message = new OutboxMessage(
            Guid.NewGuid(),
            JsonConvert.SerializeObject(new TestDomainEvent()),
            typeof(TestModule).Assembly);

        // Act
        var result = await processor.ProcessMessageAsync(message);

        // Assert
        result.Success.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task ProcessMessageAsync_DeserializationFails_ReturnsFailure()
    {
        // Arrange
        var processor = new OutboxMessageProcessor(...);
        var message = new OutboxMessage(Guid.NewGuid(), "invalid-json", ...);

        // Act
        var result = await processor.ProcessMessageAsync(message);

        // Assert
        result.Success.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
    }
}
```

**Integration Test Example**:
```csharp
public class ProcessOutboxJobIntegrationTests : IClassFixture<PostgresTestFixture>
{
    [Fact]
    public async Task Execute_WithUnprocessedMessages_ProcessesAllMessages()
    {
        // Arrange - Insert test outbox messages into database
        await using var connection = await OpenConnectionAsync();
        await InsertOutboxMessageAsync(connection, new OutboxMessage(...));
        
        var job = new TestProcessOutboxJob(...);

        // Act
        await job.Execute(new MockJobExecutionContext());

        // Assert - Verify messages were processed
        var processedMessages = await GetProcessedMessagesAsync(connection);
        processedMessages.ShouldNotBeEmpty();
        processedMessages.All(m => m.ProcessedAtUtc != null).ShouldBeTrue();
    }
}
```

**Benefits**:
- ✅ Message processing logic can be unit tested independently
- ✅ Database operations can be tested with Testcontainers
- ✅ Easier to mock and test error scenarios
- ✅ Single Responsibility Principle - job orchestrates, processor handles logic

---

### 4. Cache Service (LOW PRIORITY)

#### **Issue: IDistributedCache Dependency**

**Location**: `src/api/common/TavernTrashers.Api.Common.Infrastructure/Caching/CacheService.cs`

**Problem**:
```csharp
internal sealed class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var bytes = await cache.GetAsync(key, cancellationToken);
        return bytes is null ? default : Deserialize<T>(bytes);
    }
}
```

**Testability Issues**:
- While `IDistributedCache` is an interface, testing serialization/deserialization logic requires setup
- No validation of cache keys or expiration policies
- Serialization errors are not handled

**Proposed Refactor**:

**Option A: In-Memory Test Implementation**

```csharp
// New file: src/api/common/TavernTrashers.Api.Common.Infrastructure.Tests/Caching/InMemoryDistributedCache.cs
public sealed class InMemoryDistributedCache : IDistributedCache
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

    private sealed record CacheEntry(byte[] Value, DateTimeOffset? AbsoluteExpiration);

    public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.AbsoluteExpiration.HasValue && entry.AbsoluteExpiration < DateTimeOffset.UtcNow)
            {
                _cache.TryRemove(key, out _);
                return Task.FromResult<byte[]?>(null);
            }
            return Task.FromResult<byte[]?>(entry.Value);
        }
        return Task.FromResult<byte[]?>(null);
    }

    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        var entry = new CacheEntry(value, options.AbsoluteExpiration);
        _cache[key] = entry;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    // Other interface methods...
}

// Unit test
public class CacheServiceTests
{
    [Fact]
    public async Task GetAsync_CachedValue_ReturnsDeserializedValue()
    {
        // Arrange
        var cache = new InMemoryDistributedCache();
        var service = new CacheService(cache);
        
        var testObject = new TestData { Id = 123, Name = "Test" };
        await service.SetAsync("test-key", testObject);

        // Act
        var result = await service.GetAsync<TestData>("test-key");

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(123);
        result.Name.ShouldBe("Test");
    }

    [Fact]
    public async Task GetAsync_ExpiredValue_ReturnsNull()
    {
        // Arrange
        var cache = new InMemoryDistributedCache();
        var service = new CacheService(cache);
        
        await service.SetAsync("test-key", "value", TimeSpan.FromMilliseconds(1));
        await Task.Delay(10);

        // Act
        var result = await service.GetAsync<string>("test-key");

        // Assert
        result.ShouldBeNull();
    }
}
```

**Option B: Testcontainers Redis Integration**

```csharp
public class CacheServiceIntegrationTests : IClassFixture<RedisTestFixture>
{
    private readonly RedisTestFixture _fixture;

    public CacheServiceIntegrationTests(RedisTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SetAsync_ComplexObject_CanBeRetrieved()
    {
        // Arrange
        var cache = _fixture.CreateDistributedCache();
        var service = new CacheService(cache);
        
        var roll = new Roll { Id = Guid.NewGuid(), Expression = "1d20", Value = 15 };

        // Act
        await service.SetAsync($"roll:{roll.Id}", roll);
        var retrieved = await service.GetAsync<Roll>($"roll:{roll.Id}");

        // Assert
        retrieved.ShouldNotBeNull();
        retrieved.Id.ShouldBe(roll.Id);
        retrieved.Expression.ShouldBe(roll.Expression);
    }
}

public sealed class RedisTestFixture : IAsyncLifetime
{
    private readonly RedisContainer _redis = new RedisBuilder()
        .WithImage("redis:7-alpine")
        .Build();

    public async Task InitializeAsync() => await _redis.StartAsync();
    
    public async Task DisposeAsync() => await _redis.DisposeAsync();

    public IDistributedCache CreateDistributedCache()
    {
        var configuration = ConfigurationOptions.Parse(_redis.GetConnectionString());
        var connection = ConnectionMultiplexer.Connect(configuration);
        return new RedisCache(new RedisCacheOptions
        {
            ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(connection)
        });
    }
}
```

**Benefits**:
- ✅ Unit tests don't require Redis
- ✅ Integration tests verify actual Redis behavior
- ✅ Serialization edge cases can be tested
- ✅ Cache expiration logic is testable

---

### 5. Pipeline Behaviors (MEDIUM PRIORITY)

#### **Issue: UnitOfWorkSaveChangesPipelineBehavior Service Locator**

**Location**: `src/api/common/TavernTrashers.Api.Common.Application/Behaviors/UnitOfWorkSaveChangesPipelineBehavior.cs`

**Problem**:
```csharp
internal sealed class UnitOfWorkSaveChangesPipelineBehavior<TRequest, TResponse>(
    IServiceProvider serviceProvider) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);
        if (response.IsFailure) return response;

        var moduleName = request.GetModuleName();
        var unitOfWork = serviceProvider.GetRequiredKeyedService<IUnitOfWork>(moduleName);  // Service Locator
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }
}
```

**Testability Issues**:
- Service Locator pattern makes it hard to verify correct UnitOfWork is resolved
- Difficult to mock `IServiceProvider`
- Can't easily test what happens when module name is incorrect
- No way to verify SaveChangesAsync was called on the right instance

**Proposed Refactor**:

**Option A: Extract Module Context Resolver**

```csharp
// New abstraction
public interface IModuleContextResolver
{
    IUnitOfWork ResolveUnitOfWork(string moduleName);
}

// Implementation
internal sealed class ModuleContextResolver(IServiceProvider serviceProvider) : IModuleContextResolver
{
    public IUnitOfWork ResolveUnitOfWork(string moduleName)
    {
        try
        {
            return serviceProvider.GetRequiredKeyedService<IUnitOfWork>(moduleName);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"No UnitOfWork registered for module '{moduleName}'", ex);
        }
    }
}

// Refactored behavior
internal sealed class UnitOfWorkSaveChangesPipelineBehavior<TRequest, TResponse>(
    IModuleContextResolver moduleContextResolver) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);
        if (response.IsFailure) return response;

        var moduleName = request.GetModuleName();
        var unitOfWork = moduleContextResolver.ResolveUnitOfWork(moduleName);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }
}
```

**Unit Test**:
```csharp
public class UnitOfWorkSaveChangesPipelineBehaviorTests
{
    [Fact]
    public async Task Handle_SuccessfulCommand_CallsSaveChanges()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var resolverMock = new Mock<IModuleContextResolver>();
        resolverMock.Setup(r => r.ResolveUnitOfWork("dice"))
            .Returns(unitOfWorkMock.Object);

        var behavior = new UnitOfWorkSaveChangesPipelineBehavior<TestCommand, Result>(
            resolverMock.Object);

        var request = new TestCommand("dice");
        RequestHandlerDelegate<Result> next = () => Task.FromResult(Result.Success());

        // Act
        await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FailedCommand_DoesNotCallSaveChanges()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var resolverMock = new Mock<IModuleContextResolver>();
        
        var behavior = new UnitOfWorkSaveChangesPipelineBehavior<TestCommand, Result>(resolverMock.Object);

        RequestHandlerDelegate<Result> next = () => Task.FromResult(Result.Failure(new Error("test.error", "Test error")));

        // Act
        await behavior.Handle(new TestCommand("dice"), next, CancellationToken.None);

        // Assert
        resolverMock.Verify(r => r.ResolveUnitOfWork(It.IsAny<string>()), Times.Never);
        unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
```

**Benefits**:
- ✅ Service Locator is isolated in a testable component
- ✅ Behavior logic can be unit tested with mocks
- ✅ Error cases (missing module) can be tested
- ✅ Verification of SaveChanges calls is straightforward

---

### 6. Permission Service (LOW PRIORITY)

#### **Issue: PermissionService Thin Wrapper**

**Location**: `src/api/modules/users/TavernTrashers.Api.Modules.Users.Infrastructure/Authorization/PermissionService.cs`

**Problem**:
```csharp
internal sealed class PermissionService(ISender sender) : IPermissionService
{
    public async Task<Result<PermissionResponse>> GetUserPermissionsAsync(string identityId) =>
        await sender.Send(new GetUserPermissionsQuery(identityId));
}
```

**Testability Issues**:
- Thin wrapper around MediatR, adds little value
- Testing it requires mocking ISender
- Doesn't validate input or add any logic
- Creates unnecessary abstraction layer

**Proposed Refactor**:

**Option A: Remove Unnecessary Abstraction**

```csharp
// Remove PermissionService entirely

// In authorization code, use ISender directly:
internal sealed class CustomClaimsTransformation(
    ISender sender,  // Use ISender directly instead of IPermissionService
    ILogger<CustomClaimsTransformation> logger) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Send query directly
        var result = await sender.Send(new GetUserPermissionsQuery(identityId));
        // ...
    }
}
```

**Option B: Add Validation and Caching** (if keeping the service)

```csharp
internal sealed class PermissionService(
    ISender sender,
    ICacheService cache,
    ILogger<PermissionService> logger) : IPermissionService
{
    public async Task<Result<PermissionResponse>> GetUserPermissionsAsync(string identityId)
    {
        if (string.IsNullOrWhiteSpace(identityId))
        {
            return Result.Failure<PermissionResponse>(
                new Error("permission.invalid_id", "Identity ID cannot be empty"));
        }

        // Check cache first
        var cacheKey = $"permissions:{identityId}";
        var cached = await cache.GetAsync<PermissionResponse>(cacheKey);
        if (cached is not null)
        {
            return Result.Success(cached);
        }

        // Query and cache
        var result = await sender.Send(new GetUserPermissionsQuery(identityId));
        if (result.IsSuccess)
        {
            await cache.SetAsync(cacheKey, result.Value, TimeSpan.FromMinutes(5));
        }

        return result;
    }
}
```

**Unit Test**:
```csharp
public class PermissionServiceTests
{
    [Fact]
    public async Task GetUserPermissionsAsync_EmptyIdentityId_ReturnsFailure()
    {
        // Arrange
        var service = new PermissionService(Mock.Of<ISender>(), Mock.Of<ICacheService>(), NullLogger<PermissionService>.Instance);

        // Act
        var result = await service.GetUserPermissionsAsync(string.Empty);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("permission.invalid_id");
    }

    [Fact]
    public async Task GetUserPermissionsAsync_CachedPermissions_ReturnsCached()
    {
        // Arrange
        var cacheMock = new Mock<ICacheService>();
        var cachedPermissions = new PermissionResponse(["read", "write"]);
        cacheMock.Setup(c => c.GetAsync<PermissionResponse>("permissions:user123", default))
            .ReturnsAsync(cachedPermissions);

        var senderMock = new Mock<ISender>();
        var service = new PermissionService(senderMock.Object, cacheMock.Object, NullLogger<PermissionService>.Instance);

        // Act
        var result = await service.GetUserPermissionsAsync("user123");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(cachedPermissions);
        senderMock.Verify(s => s.Send(It.IsAny<GetUserPermissionsQuery>(), default), Times.Never);
    }
}
```

**Benefits**:
- ✅ Either eliminates unnecessary layer or adds real value
- ✅ Validation logic can be tested
- ✅ Caching behavior can be verified
- ✅ Clearer separation of concerns

---

## Implementation Priority

### Phase 1: Critical Infrastructure (Week 1)
1. **Add Testcontainers support for PostgreSQL**
   - Add package: `Testcontainers.PostgreSql`
   - Create `PostgresTestFixture` base class
   - Add to `Directory.Packages.props`

2. **Extract HttpClient abstractions**
   - Create `IKeyCloakClient` and `IKeyCloakTokenClient` interfaces
   - Update dependency injection registration
   - Add integration test using WireMock or Testcontainers

3. **Create test data builders**
   - RollBuilder for Dice module
   - CharacterBuilder for Characters module
   - UserBuilder for Users module

### Phase 2: Background Processing (Week 2)
4. **Refactor Outbox/Inbox jobs**
   - Extract `IOutboxMessageProcessor` interface
   - Add unit tests for message processing
   - Add integration tests with Testcontainers

5. **Improve pipeline behaviors**
   - Extract `IModuleContextResolver`
   - Add comprehensive unit tests
   - Document testing patterns

### Phase 3: Supporting Infrastructure (Week 3)
6. **Add Cache testing infrastructure**
   - Create `InMemoryDistributedCache` for unit tests
   - Add Testcontainers Redis support for integration tests

7. **Review and simplify abstractions**
   - Evaluate thin wrapper services like PermissionService
   - Add validation and caching where appropriate
   - Remove unnecessary abstractions

---

## Recommended Testing Packages

Add to `Directory.Packages.props`:

```xml
<!-- Testing Infrastructure -->
<PackageVersion Include="Moq" Version="4.20.70" />
<PackageVersion Include="Testcontainers" Version="3.9.0" />
<PackageVersion Include="Testcontainers.PostgreSql" Version="3.9.0" />
<PackageVersion Include="Testcontainers.Redis" Version="3.9.0" />
<PackageVersion Include="Testcontainers.RabbitMq" Version="3.9.0" />
<PackageVersion Include="WireMock.Net" Version="1.5.62" />
<PackageVersion Include="FluentAssertions" Version="6.12.1" /> <!-- Optional alternative to Shouldly -->
```

---

## Testing Guidelines Document

Create `docs/TESTING_GUIDELINES.md`:

### Unit Testing
- Use Moq for mocking dependencies
- One assertion per test (when possible)
- Use test data builders for complex objects
- Follow AAA pattern (Arrange, Act, Assert)
- Use meaningful test names: `MethodName_Scenario_ExpectedResult`

### Integration Testing
- Use Testcontainers for external dependencies
- Share fixtures across test classes with `IClassFixture`
- Clean up test data between tests
- Use realistic test scenarios
- Test edge cases and error conditions

### Test Organization
```
Module.Tests/
├── Unit/                    # Fast, isolated tests
│   ├── Domain/
│   ├── Application/
│   └── Infrastructure/
├── Integration/             # Tests with real dependencies
│   ├── Database/
│   ├── Http/
│   └── Messaging/
└── Fixtures/                # Shared test infrastructure
    ├── PostgresTestFixture.cs
    └── RedisTestFixture.cs
```

---

## Benefits of Proposed Refactorings

### Testability
- ✅ Unit tests can run without external dependencies
- ✅ Integration tests use real infrastructure via containers
- ✅ Clear separation between unit and integration test concerns
- ✅ Faster feedback loop for developers

### Maintainability
- ✅ Interfaces make dependencies explicit
- ✅ Test data builders reduce boilerplate
- ✅ Shared fixtures promote consistency
- ✅ Better error messages and diagnostics

### Quality
- ✅ Higher test coverage possible
- ✅ Edge cases easier to test
- ✅ Confidence in refactoring
- ✅ Regression detection improved

---

## Conclusion

The codebase follows clean architecture principles well, but lacks comprehensive test infrastructure. The proposed refactorings focus on:

1. **Introducing abstractions** where direct dependencies hinder testing
2. **Creating test infrastructure** (Testcontainers, fixtures, builders)
3. **Extracting complex logic** into testable components
4. **Simplifying or removing** unnecessary abstraction layers

These changes will enable both fast unit tests and reliable integration tests, significantly improving the overall quality and maintainability of the codebase.

# Testing Guidelines

This document provides guidelines and best practices for testing the Tavern Trashers backend codebase.

## Table of Contents

1. [Testing Philosophy](#testing-philosophy)
2. [Test Organization](#test-organization)
3. [Unit Testing](#unit-testing)
4. [Integration Testing](#integration-testing)
5. [Test Fixtures](#test-fixtures)
6. [Naming Conventions](#naming-conventions)
7. [Common Patterns](#common-patterns)

---

## Testing Philosophy

- **Test Pyramid**: Prioritize unit tests over integration tests, and integration tests over end-to-end tests
- **Fast Feedback**: Unit tests should run in milliseconds, integration tests in seconds
- **Isolated**: Tests should not depend on each other or external state
- **Readable**: Tests should be easy to understand and maintain
- **Maintainable**: Avoid brittle tests that break with minor refactorings

---

## Test Organization

### Directory Structure

```
Module.Tests/
├── Unit/                           # Fast, isolated tests
│   ├── Domain/                     # Domain logic tests
│   ├── Application/                # Command/Query handler tests
│   └── Infrastructure/             # Infrastructure service tests
├── Integration/                    # Tests with real dependencies
│   ├── Database/                   # Repository integration tests
│   ├── Http/                       # HTTP client integration tests
│   └── Messaging/                  # Message bus integration tests
└── Fixtures/                       # Shared test infrastructure
    ├── PostgresTestFixture.cs      # Database test fixture
    └── TestDataBuilders.cs         # Test data builders
```

### Project References

```xml
<ItemGroup>
    <!-- Core testing packages -->
    <PackageReference Include="xunit"/>
    <PackageReference Include="Shouldly"/>
    <PackageReference Include="Bogus"/>
    
    <!-- Integration testing -->
    <PackageReference Include="Testcontainers.PostgreSql"/>
    <PackageReference Include="Testcontainers.Redis"/>
    <PackageReference Include="Testcontainers.RabbitMq"/>
    
    <!-- Mocking (when needed) -->
    <PackageReference Include="Moq"/>
    
    <!-- HTTP mocking -->
    <PackageReference Include="WireMock.Net"/>
</ItemGroup>
```

---

## Unit Testing

### Principles

- **No external dependencies**: Use mocks, stubs, or in-memory implementations
- **Fast**: Should complete in milliseconds
- **Deterministic**: Same input always produces same output
- **Single responsibility**: Test one thing at a time

### AAA Pattern

All tests should follow the Arrange-Act-Assert pattern:

```csharp
[Fact]
public void Handle_ValidCommand_ReturnsSuccess()
{
    // Arrange - Set up test data and dependencies
    var command = new RollDiceCommand("1d20");
    var handler = new RollDiceCommandHandler(...);

    // Act - Execute the code under test
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert - Verify the outcome
    result.IsSuccess.ShouldBeTrue();
    result.Value.ShouldNotBeNull();
}
```

### Testing with Result Pattern

```csharp
[Fact]
public void ParseExpression_InvalidExpression_ReturnsFailure()
{
    // Arrange
    var parser = new DiceExpressionParser();
    var invalidExpression = "invalid";

    // Act
    var result = parser.ParseExpression(invalidExpression);

    // Assert
    result.IsFailure.ShouldBeTrue();
    result.Error.Code.ShouldBe("dice.invalid_expression");
}
```

### Using Test Data Builders

When domain objects are complex, use builders:

```csharp
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

    public Roll Build() => Roll.Create(_id, _expression, _value, _parent);
}

// Usage in tests
[Fact]
public void Reroll_RollWithParent_CreatesChildRoll()
{
    // Arrange
    var parent = new RollBuilder()
        .WithExpression("1d20")
        .Build();

    var child = new RollBuilder()
        .WithParent(parent)
        .Build();

    // Act & Assert
    child.Parent.ShouldBe(parent);
}
```

### Mocking Dependencies

Use Moq for mocking interfaces when necessary:

```csharp
[Fact]
public async Task Handle_SuccessfulCommand_CallsSaveChanges()
{
    // Arrange
    var unitOfWorkMock = new Mock<IUnitOfWork>();
    var handler = new CommandHandler(unitOfWorkMock.Object);

    // Act
    await handler.Handle(new Command(), CancellationToken.None);

    // Assert
    unitOfWorkMock.Verify(
        uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), 
        Times.Once);
}
```

---

## Integration Testing

### Principles

- **Real dependencies**: Use actual databases, message queues, etc.
- **Testcontainers**: Run dependencies in Docker containers
- **Clean state**: Each test should start with a clean database
- **Realistic scenarios**: Test actual integration points

### Using PostgreSQL Test Fixture

```csharp
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
        // Arrange - Create DbContext with test connection string
        await using var context = CreateDbContext(_fixture.ConnectionString);
        await context.Database.EnsureCreatedAsync();
        
        var repository = new RollRepository(context);
        var roll = Roll.Create(Guid.NewGuid(), "1d20", 15);
        
        context.Rolls.Add(roll);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAsync(roll.Id);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(roll.Id);
        
        // Cleanup
        await context.Database.EnsureDeletedAsync();
    }

    private static DiceDbContext CreateDbContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<DiceDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new DiceDbContext(options);
    }
}
```

### Testing HTTP Clients with WireMock

For testing services that make HTTP calls:

```csharp
public class KeyCloakClientIntegrationTests : IAsyncLifetime
{
    private WireMockServer _mockServer = null!;
    private IKeyCloakClient _client = null!;

    public async Task InitializeAsync()
    {
        _mockServer = WireMockServer.Start();
        
        var httpClient = new HttpClient 
        { 
            BaseAddress = new Uri(_mockServer.Urls[0]) 
        };
        
        _client = new KeyCloakClient(httpClient);
        
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _mockServer.Stop();
        _mockServer.Dispose();
        await Task.CompletedTask;
    }

    [Fact]
    public async Task RegisterUserAsync_SuccessfulRegistration_ReturnsUserId()
    {
        // Arrange
        var expectedUserId = "test-user-123";
        
        _mockServer
            .Given(Request.Create()
                .WithPath("/users")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithHeader("Location", $"/users/{expectedUserId}"));

        var user = new UserRepresentation(
            "test@example.com",
            "test@example.com",
            "Test",
            "User",
            true,
            true,
            [new CredentialRepresentation("password", "secret", false)]);

        // Act
        var result = await _client.RegisterUserAsync(user);

        // Assert
        result.ShouldBe(expectedUserId);
    }
}
```

---

## Test Fixtures

### Shared Fixtures with IClassFixture

Use `IClassFixture<T>` when multiple tests need the same expensive setup:

```csharp
public class PostgresTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("tavern_trashers_test")
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
```

### Per-Test Setup with Constructor

```csharp
public class MyTests
{
    private readonly ILogger<MyService> _logger;
    private readonly MyService _service;

    public MyTests()
    {
        _logger = new NullLogger<MyService>();
        _service = new MyService(_logger);
    }

    [Fact]
    public void Test1() { /* ... */ }

    [Fact]
    public void Test2() { /* ... */ }
}
```

---

## Naming Conventions

### Test Class Names

- Format: `{ClassUnderTest}Tests`
- Examples: `RollRepositoryTests`, `DiceExpressionParserTests`

### Test Method Names

- Format: `{MethodName}_{Scenario}_{ExpectedResult}`
- Use underscores for readability
- Be descriptive and specific

Examples:
```csharp
[Fact]
public void ParseExpression_ValidDiceNotation_ReturnsSuccessResult()

[Fact]
public void Handle_InvalidCommand_ReturnsValidationError()

[Fact]
public async Task GetAsync_NonExistentRoll_ReturnsNotFoundError()
```

### Theory Data Names

```csharp
public static IEnumerable<object[]> ValidDiceExpressions => new List<object[]>
{
    new object[] { "1d20", 1, 20 },
    new object[] { "2d6", 2, 12 },
    new object[] { "4d6kh3", 3, 18 },
};

[Theory]
[MemberData(nameof(ValidDiceExpressions))]
public void ParseExpression_ValidInput_ParsesCorrectly(
    string expression, 
    int expectedMin, 
    int expectedMax)
{
    // ...
}
```

---

## Common Patterns

### Testing Async Methods

```csharp
[Fact]
public async Task Handle_ValidCommand_CompletesSuccessfully()
{
    // Arrange
    var command = new TestCommand();
    var handler = new TestCommandHandler();

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.ShouldBeTrue();
}
```

### Testing Exceptions

```csharp
[Fact]
public void Constructor_NullArgument_ThrowsArgumentNullException()
{
    // Act & Assert
    Should.Throw<ArgumentNullException>(() => new MyClass(null!));
}
```

### Testing Collections

```csharp
[Fact]
public async Task GetAll_WithMultipleRolls_ReturnsAllRolls()
{
    // Arrange
    var rolls = new List<Roll>
    {
        new RollBuilder().Build(),
        new RollBuilder().Build(),
        new RollBuilder().Build()
    };

    // Act
    var result = await repository.GetAllAsync();

    // Assert
    result.ShouldNotBeEmpty();
    result.Count.ShouldBe(3);
    result.ShouldAllBe(r => r.Id != Guid.Empty);
}
```

### Testing with Fake Data

```csharp
public class MyTests : TestBase  // Inherits Faker
{
    [Fact]
    public void Test_WithRandomData()
    {
        // Arrange - Use inherited Faker
        var email = Faker.Internet.Email();
        var name = Faker.Name.FullName();
        var age = Faker.Random.Int(18, 100);

        // Act & Assert
        // ...
    }
}
```

### Data-Driven Tests

```csharp
[Theory]
[InlineData("1d20", 1, 20)]
[InlineData("2d6", 2, 12)]
[InlineData("4d6", 4, 24)]
public void ParseExpression_ValidInput_ReturnsCorrectBounds(
    string expression,
    int expectedMin,
    int expectedMax)
{
    // ...
}
```

---

## Best Practices

### DO

✅ Write tests before fixing bugs (TDD for bug fixes)  
✅ Test edge cases and boundary conditions  
✅ Use descriptive test names that explain the scenario  
✅ Keep tests simple and focused  
✅ Use test data builders for complex objects  
✅ Clean up resources in tests (database, files, etc.)  
✅ Use `IAsyncLifetime` for async setup/teardown  
✅ Mock only what you need to control  

### DON'T

❌ Test private methods directly (test through public API)  
❌ Have tests depend on execution order  
❌ Use `Thread.Sleep` (use proper async/await)  
❌ Test framework internals (test your code, not EF Core)  
❌ Share mutable state between tests  
❌ Ignore failing tests  
❌ Write tests that are harder to understand than the code  

---

## Running Tests

### Run all tests
```bash
dotnet test src/TavernTrashers.slnx
```

### Run specific test project
```bash
dotnet test src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Domain.Tests
```

### Run tests with filter
```bash
dotnet test --filter "FullyQualifiedName~DiceParser"
```

### Run tests with code coverage
```bash
dotnet test src/TavernTrashers.slnx --collect:"XPlat Code Coverage"
```

---

## Additional Resources

- [xUnit Documentation](https://xunit.net/)
- [Shouldly Documentation](https://github.com/shouldly/shouldly)
- [Testcontainers Documentation](https://dotnet.testcontainers.org/)
- [Moq Documentation](https://github.com/moq/moq4)
- [Bogus Documentation](https://github.com/bchavez/Bogus)

---

## Conclusion

Following these guidelines will help ensure the codebase maintains high test coverage with tests that are:
- **Fast** to run
- **Easy** to understand
- **Reliable** and deterministic
- **Maintainable** over time

Remember: Good tests are an investment in the future maintainability and reliability of the codebase.

# Testing Guide

Comprehensive testing strategy and guidelines for Tavern Trashers.

## Table of Contents

- [Testing Philosophy](#testing-philosophy)
- [Test Types](#test-types)
- [Running Tests](#running-tests)
- [Writing Tests](#writing-tests)
- [Test Organization](#test-organization)
- [Testing Patterns](#testing-patterns)
- [Code Coverage](#code-coverage)
- [Continuous Integration](#continuous-integration)

## Testing Philosophy

Our testing strategy follows the **Testing Pyramid**:

```
        /\
       /  \      E2E Tests (Few)
      /____\
     /      \    Integration Tests (Some)
    /________\
   /          \  Unit Tests (Many)
  /____________\
```

### Principles

1. **Fast Feedback**: Tests should run quickly
2. **Reliable**: Tests should be deterministic
3. **Isolated**: Tests should not depend on each other
4. **Maintainable**: Tests should be easy to understand and update
5. **Valuable**: Tests should provide confidence in the system

## Test Types

### Unit Tests

Test individual components in isolation.

**Characteristics**:
- Fast execution (< 100ms per test)
- No external dependencies
- High code coverage
- Focus on business logic

**Examples**:
- Domain entity validation
- Value object behavior
- Utility functions
- Business rule enforcement

### Integration Tests

Test components working together.

**Characteristics**:
- Slower than unit tests
- May use test databases or in-memory providers
- Test infrastructure integration
- Verify component interactions

**Examples**:
- Repository operations
- API endpoint behavior
- Message handling
- Authentication flow

### End-to-End Tests

Test complete user workflows.

**Characteristics**:
- Slowest execution
- Full system deployment
- Real browser/API interactions
- Test user scenarios

**Examples**:
- User registration and login
- Rolling dice and viewing history
- Character creation workflow
- Campaign management

## Running Tests

### All Tests

```bash
# Run all tests
dotnet test src/TavernTrashers.slnx

# Run with detailed output
dotnet test --verbosity detailed

# Run in parallel
dotnet test --parallel
```

### Specific Test Projects

```bash
# Domain tests
dotnet test src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Domain.Tests

# Common infrastructure tests
dotnet test src/api/common/TavernTrashers.Api.Common.Infrastructure.Tests
```

### Filtered Tests

```bash
# By category
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"

# By test name
dotnet test --filter "FullyQualifiedName~RollDice"

# By namespace
dotnet test --filter "FullyQualifiedName~Dice.Domain"
```

### Frontend Tests

```bash
cd src/web/tavern-trashers-web

# Unit tests
npm test

# Unit tests with coverage
npm run test:coverage

# E2E tests
npm run e2e

# Watch mode
npm run test:watch
```

## Writing Tests

### Unit Test Structure

Follow **AAA Pattern** (Arrange-Act-Assert):

```csharp
public class RollTests
{
    [Fact]
    public void Create_WithValidExpression_ShouldSucceed()
    {
        // Arrange
        var expression = "2d20+5";
        var rollOutcome = new RollOutcome(
            Total: 27,
            Minimum: 7,
            Maximum: 45,
            Average: 26.0,
            RawRolls: new List<DieResult>(),
            KeptRolls: new List<DieResult>()
        );
        var rolledAtUtc = DateTime.UtcNow;
        var contextJson = "{}";

        // Act
        var result = Roll.Create(expression, rollOutcome, rolledAtUtc, contextJson);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Expression.Should().Be(expression);
        result.Value.Total.Should().Be(27);
    }
}
```

### Test Naming Conventions

**Pattern**: `MethodName_Scenario_ExpectedBehavior`

**Examples**:
- `Create_WithNullExpression_ShouldReturnError`
- `RollDice_WithValidNotation_ShouldReturnRollOutcome`
- `GetRoll_WithNonExistentId_ShouldReturnNotFoundError`

### Using FluentAssertions

```csharp
// Instead of Assert
Assert.Equal(expected, actual);
Assert.True(condition);

// Use FluentAssertions
actual.Should().Be(expected);
condition.Should().BeTrue();

// More readable assertions
result.Should().NotBeNull();
result.IsSuccess.Should().BeTrue();
result.Value.Should().BeOfType<Roll>();
collection.Should().HaveCount(5);
exception.Should().BeOfType<ArgumentException>();
```

### Testing Async Code

```csharp
[Fact]
public async Task GetRollAsync_WithValidId_ShouldReturnRoll()
{
    // Arrange
    var rollId = Guid.NewGuid();
    var repository = new Mock<IRollRepository>();
    repository.Setup(r => r.GetByIdAsync(rollId, default))
        .ReturnsAsync(new Roll(...));

    // Act
    var result = await repository.Object.GetByIdAsync(rollId);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(rollId);
}
```

### Mocking with Moq

```csharp
[Fact]
public void RollDice_ShouldCallRepository()
{
    // Arrange
    var mockRepository = new Mock<IRollRepository>();
    var handler = new RollDiceHandler(mockRepository.Object);
    
    var command = new RollDiceCommand("2d20");

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    mockRepository.Verify(r => 
        r.AddAsync(It.IsAny<Roll>(), It.IsAny<CancellationToken>()), 
        Times.Once);
}
```

### Testing Error Cases

```csharp
[Fact]
public void Create_WithEmptyExpression_ShouldReturnValidationError()
{
    // Arrange
    var expression = string.Empty;
    
    // Act
    var result = Roll.Create(expression, ...);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Type.Should().Be(ErrorType.Validation);
    result.Error.Code.Should().Be("Roll.EmptyExpression");
}
```

### Testing Domain Events

```csharp
[Fact]
public void Create_ShouldRaiseDomainEvent()
{
    // Arrange
    var expression = "2d20";

    // Act
    var result = Roll.Create(expression, ...);

    // Assert
    result.Value.DomainEvents.Should().ContainSingle(e => 
        e is RollCreatedEvent);
}
```

## Test Organization

### Project Structure

```
TavernTrashers.Api.Modules.Dice.Domain.Tests/
├── Rolls/
│   ├── RollTests.cs
│   └── RollOutcomeTests.cs
├── DiceEngine/
│   ├── DiceRollerTests.cs
│   └── DiceNotationParserTests.cs
├── Builders/
│   ├── RollBuilder.cs
│   └── RollOutcomeBuilder.cs
└── Fixtures/
    └── RollFixture.cs
```

### Test Data Builders

```csharp
public class RollBuilder
{
    private string _expression = "2d20";
    private int _total = 20;
    private DateTime _rolledAtUtc = DateTime.UtcNow;

    public RollBuilder WithExpression(string expression)
    {
        _expression = expression;
        return this;
    }

    public RollBuilder WithTotal(int total)
    {
        _total = total;
        return this;
    }

    public Roll Build()
    {
        var rollOutcome = new RollOutcome(_total, ...);
        return Roll.Create(_expression, rollOutcome, _rolledAtUtc, "{}").Value;
    }
}

// Usage
var roll = new RollBuilder()
    .WithExpression("3d6")
    .WithTotal(15)
    .Build();
```

### Test Fixtures (xUnit)

```csharp
public class DatabaseFixture : IDisposable
{
    public DiceDbContext Context { get; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<DiceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new DiceDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}

[Collection("Database")]
public class RollRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public RollRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_ShouldPersistRoll()
    {
        // Use _fixture.Context
    }
}
```

## Testing Patterns

### Parameterized Tests

```csharp
[Theory]
[InlineData("d20", 1, 20)]
[InlineData("2d6", 2, 12)]
[InlineData("3d8+5", 8, 29)]
public void ParseExpression_WithValidNotation_ShouldReturnCorrectRange(
    string expression, 
    int expectedMin, 
    int expectedMax)
{
    // Arrange
    var parser = new DiceParser(expression);

    // Act
    var result = parser.ParseExpression();

    // Assert
    result.IsSuccess.Should().BeTrue();
    // Additional assertions
}
```

### Test Data from Class

```csharp
public class RollExpressionTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "d20", 1, 20 };
        yield return new object[] { "2d6", 2, 12 };
        yield return new object[] { "3d8+5", 8, 29 };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

[Theory]
[ClassData(typeof(RollExpressionTestData))]
public void ParseExpression_WithValidNotation_ShouldReturnCorrectRange(
    string expression, 
    int min, 
    int max)
{
    // Test implementation
}
```

### Integration Test Setup

```csharp
public class IntegrationTestBase : IAsyncLifetime
{
    protected WebApplicationFactory<Program> Factory { get; private set; }
    protected HttpClient Client { get; private set; }

    public async Task InitializeAsync()
    {
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace services for testing
                });
            });

        Client = Factory.CreateClient();
        await SeedDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanupDatabaseAsync();
        Client?.Dispose();
        Factory?.Dispose();
    }
}
```

## Code Coverage

### Generating Coverage Reports

```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML report
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coverage-report" \
  -reporttypes:Html

# Open report
open coverage-report/index.html
```

### Coverage Goals

- **Domain Layer**: 90%+ coverage
- **Application Layer**: 80%+ coverage
- **Infrastructure Layer**: 70%+ coverage
- **Presentation Layer**: 60%+ coverage

### Excluding from Coverage

```csharp
[ExcludeFromCodeCoverage]
public class GeneratedCode
{
    // Auto-generated code
}
```

## Frontend Testing

### Component Tests

```typescript
describe('DiceRollerComponent', () => {
  let component: DiceRollerComponent;
  let fixture: ComponentFixture<DiceRollerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DiceRollerComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(DiceRollerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should roll dice when button clicked', () => {
    const button = fixture.nativeElement.querySelector('button');
    button.click();
    
    expect(component.lastRoll).toBeDefined();
  });
});
```

### Service Tests

```typescript
describe('DiceService', () => {
  let service: DiceService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [DiceService]
    });

    service = TestBed.inject(DiceService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should roll dice', () => {
    const mockRoll = { total: 15, expression: '2d6' };

    service.rollDice('2d6').subscribe(roll => {
      expect(roll.total).toBe(15);
    });

    const req = httpMock.expectOne('/api/dice/rolls');
    expect(req.request.method).toBe('POST');
    req.flush(mockRoll);
  });
});
```

## Continuous Integration

### GitHub Actions Workflow

Tests run automatically on:
- Pull requests
- Pushes to main
- Scheduled nightly builds

### CI Test Commands

```yaml
# .github/workflows/test.yml
- name: Run Tests
  run: |
    dotnet test --no-build --verbosity normal --logger trx
    
- name: Upload Test Results
  uses: actions/upload-artifact@v3
  with:
    name: test-results
    path: "**/*.trx"
```

## Best Practices

1. **Write tests first** (TDD when appropriate)
2. **One assertion per test** (generally)
3. **Test behavior, not implementation**
4. **Keep tests simple and readable**
5. **Use meaningful test names**
6. **Avoid test interdependencies**
7. **Mock external dependencies**
8. **Test edge cases and error conditions**
9. **Keep tests fast**
10. **Maintain tests like production code**

## Common Testing Patterns

### Testing Repository Methods

```csharp
[Fact]
public async Task GetByIdAsync_WithExistingId_ShouldReturnRoll()
{
    // Arrange
    var roll = new RollBuilder().Build();
    await _fixture.Context.Rolls.AddAsync(roll);
    await _fixture.Context.SaveChangesAsync();
    
    var repository = new RollRepository(_fixture.Context);

    // Act
    var result = await repository.GetByIdAsync(roll.Id);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(roll.Id);
}
```

### Testing Command Handlers

```csharp
[Fact]
public async Task Handle_WithValidCommand_ShouldCreateRoll()
{
    // Arrange
    var mockRepository = new Mock<IRollRepository>();
    var handler = new RollDiceHandler(mockRepository.Object);
    
    var command = new RollDiceCommand("2d20+5");

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();
    mockRepository.Verify(r => 
        r.AddAsync(It.IsAny<Roll>(), It.IsAny<CancellationToken>()), 
        Times.Once);
}
```

### Testing API Endpoints

```csharp
[Fact]
public async Task Post_RollDice_ShouldReturn201Created()
{
    // Arrange
    var request = new { expression = "2d20" };

    // Act
    var response = await _client.PostAsJsonAsync("/api/dice/rolls", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    
    var roll = await response.Content.ReadFromJsonAsync<RollResponse>();
    roll.Should().NotBeNull();
    roll.Expression.Should().Be("2d20");
}
```

## Debugging Tests

### Visual Studio
1. Set breakpoint in test
2. Right-click test → Debug Test

### VS Code
1. Install .NET Test Explorer extension
2. Click debug icon next to test

### Command Line
```bash
# Run specific test with debugger
dotnet test --filter "FullyQualifiedName~TestName" --logger "console;verbosity=detailed"
```

## Resources

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [Moq Documentation](https://github.com/moq/moq4)
- [.NET Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [Angular Testing Guide](https://angular.dev/guide/testing)

---

*Happy testing! Quality is not an act, it is a habit.* 🧪

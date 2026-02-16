# Common Projects Unit Testing Guide

This document provides an overview of unit testing patterns and practices for the common domain, application, infrastructure, and presentation projects.

## Test Projects Structure

### Location
All common test projects are located in `src/api/common/`:

- `TavernTrashers.Api.Common.Domain.Tests` - Tests for domain layer
- `TavernTrashers.Api.Common.Application.Tests` - Tests for application layer
- `TavernTrashers.Api.Common.Infrastructure.Tests` - Tests for infrastructure layer
- `TavernTrashers.Api.Common.Presentation.Tests` - Tests for presentation layer

### Project Configuration
All test projects follow a consistent pattern:
- Target Framework: `net10.0`
- Test Framework: xUnit
- Assertion Library: Shouldly (for fluent assertions)
- Test Data Generation: Bogus (via `TestBase` class)
- Code Coverage: coverlet.collector

## Testing Patterns

### Test Structure
Tests follow the Arrange-Act-Assert (AAA) pattern:

```csharp
[Fact]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - Set up test data and dependencies
    var value = Faker.Random.Int();
    var result = Result.Success(value);

    // Act - Execute the method being tested
    var transformedResult = result.Transform(v => v.ToString());

    // Assert - Verify the outcome
    transformedResult.IsSuccess.ShouldBeTrue();
    transformedResult.Value.ShouldBe(value.ToString());
}
```

### Test Naming Convention
Tests follow the pattern: `MethodName_Scenario_ExpectedBehavior`

Examples:
- `Then_On_Success_Should_Execute_Binder`
- `Ensure_With_False_Condition_Should_Return_Error`
- `Match_On_Failure_Result_Should_Execute_OnFailure`

## Test Coverage Overview

### Domain Layer (66 tests)

#### Result Pattern (13 tests)
- Success and failure result creation
- Implicit conversions (value to result, error to result)
- Value access validation
- Validation failure handling

#### Error Factory Methods (7 tests)
- Error.None and Error.NullValue constants
- Error type factory methods: Failure, Validation, Problem, NotFound, Conflict, Authorization

#### Result Extensions
- **Then Extensions (11 tests)**: Chaining operations with value transformation
- **Match Extensions (8 tests)**: Pattern matching for success/failure cases
- **Do Extensions (8 tests)**: Side effect execution without changing the result
- **Ensure Extensions (8 tests)**: Conditional validation with error handling
- **Transform Extensions (5 tests)**: Value transformation and type conversion

#### EntityBase (6 tests)
- Domain event collection management
- Raising and clearing domain events

### Application Layer (1 test)
- Request extension for module name extraction from namespace

### Infrastructure Layer (1 test)
- DateTimeProvider UTC time validation

### Presentation Layer (9 tests)
- Result to IResult mapping (Ok, Created)
- Async result handling

## Running Tests

### Run All Tests
```bash
dotnet test src/TavernTrashers.sln
```

### Run Specific Test Project
```bash
dotnet test src/api/common/TavernTrashers.Api.Common.Domain.Tests/
```

## Test Statistics

- **Total Tests**: 77
- **Domain Tests**: 66
- **Application Tests**: 1
- **Infrastructure Tests**: 1
- **Presentation Tests**: 9
- **Pass Rate**: 100%

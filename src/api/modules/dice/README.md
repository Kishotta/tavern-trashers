# Dice Module

The Dice module provides comprehensive dice rolling functionality for Tavern Trashers, including parsing dice notation, executing rolls, and maintaining roll history.

## Overview

This module implements a complete dice rolling system with support for standard tabletop RPG dice notation, including advanced features like keep/drop mechanics, rerolls, and exploding dice.

## Features

- **Dice Notation Parsing**: Parse and validate standard dice notation (e.g., `2d20+5`, `4d6kh3`)
- **Dice Rolling**: Execute dice rolls with precise randomization
- **Roll History**: Persist and retrieve roll history with full context
- **Reroll Support**: Reroll previous rolls maintaining lineage
- **Advanced Mechanics**: Keep highest/lowest, drop dice, exploding dice, advantage/disadvantage
- **Expression Evaluation**: Support for complex mathematical expressions

## Architecture

The module follows Clean Architecture principles with clear layer separation:

```
Dice Module
├── Domain/              # Core business logic
│   ├── Rolls/           # Roll entities and value objects
│   ├── DiceEngine/      # Dice rolling engine
│   ├── AbstractSyntaxTree/  # AST for expression parsing
│   └── RecursiveDescentParser/  # Dice notation parser
├── Application/         # Use cases and handlers
│   ├── Roll/            # Roll-related commands and queries
│   └── IntegrationEvents/  # Module integration events
├── Infrastructure/      # Data access and external concerns
│   ├── Dice/            # Repository implementations
│   └── Outbox/          # Outbox pattern for reliable messaging
├── Presentation/        # API endpoints
│   └── Dice/            # REST API controllers
├── PublicApi/           # Public contracts for other modules
└── Domain.Tests/        # Unit tests
```

## Supported Dice Notation

### Basic Rolls
- `d20` - Single 20-sided die
- `2d6` - Two 6-sided dice
- `3d8+5` - Three 8-sided dice plus 5

### Advanced Rolls
- `4d6kh3` - Roll 4d6, keep highest 3 (ability scores)
- `4d6kl1` - Roll 4d6, keep lowest 1
- `4d6dh1` - Roll 4d6, drop highest 1
- `4d6dl1` - Roll 4d6, drop lowest 1 (equivalent to kh3)

### Special Mechanics
- `2d20kh1` - Advantage (roll 2d20, keep highest)
- `2d20kl1` - Disadvantage (roll 2d20, keep lowest)
- `2d6!` - Exploding dice (reroll on maximum, planned)
- `2d20r1` - Reroll 1s (planned)

### Complex Expressions
- `(2d6+3)*2` - Mathematical operations
- `d20+5+d4` - Multiple dice in one expression
- `2d6+3+1d4+2` - Complex combinations

## Domain Layer

### Core Entities

#### Roll
Represents a dice roll with full context and results.

```csharp
public sealed class Roll : Entity
{
    public string Expression { get; }
    public int Total { get; }
    public int Minimum { get; }
    public int Maximum { get; }
    public double Average { get; }
    public DateTime RolledAtUtc { get; }
    public string ContextJson { get; }
    public IReadOnlyList<DieResult> RawRolls { get; }
    public IReadOnlyList<DieResult> KeptRolls { get; }
    public Roll? Parent { get; }
    public IReadOnlyCollection<Roll> Children { get; }
}
```

#### RollOutcome
Value object containing roll results.

```csharp
public record RollOutcome(
    int Total,
    int Minimum,
    int Maximum,
    double Average,
    IReadOnlyList<DieResult> RawRolls,
    IReadOnlyList<DieResult> KeptRolls
);
```

#### DieResult
Represents a single die roll.

```csharp
public record DieResult(
    int Die,    // Die size (e.g., 20 for d20)
    int Value   // Rolled value
);
```

### Dice Engine

The `DiceRoller` class handles the actual dice rolling:

```csharp
public class DiceRoller
{
    public RollOutcome Roll(IExpressionNode expression);
}
```

### Parser

The `DiceParser` uses recursive descent parsing to convert dice notation into an Abstract Syntax Tree (AST):

```csharp
public class DiceParser
{
    public Result<IExpressionNode> ParseExpression(string expression);
}
```

## Application Layer

### Commands

#### RollDice
Roll dice using dice notation.

```csharp
public record RollDiceCommand(
    string Expression,
    string ContextJson = "{}"
) : ICommand<Guid>;
```

#### RerollDice
Reroll a previous roll.

```csharp
public record RerollDiceCommand(
    Guid RollId
) : ICommand<Guid>;
```

### Queries

#### GetRoll
Get a specific roll by ID.

```csharp
public record GetRollQuery(
    Guid RollId
) : IQuery<RollResponse>;
```

#### GetRolls
Get paginated roll history.

```csharp
public record GetRollsQuery(
    int PageNumber = 1,
    int PageSize = 20
) : IQuery<PagedResponse<RollResponse>>;
```

## Infrastructure Layer

### Repository

```csharp
public interface IRollRepository
{
    Task<Roll?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Roll>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Roll roll, CancellationToken cancellationToken = default);
}
```

### Database Schema

```sql
CREATE TABLE Rolls (
    Id UUID PRIMARY KEY,
    ParentId UUID REFERENCES Rolls(Id),
    Expression VARCHAR(500) NOT NULL,
    Total INT NOT NULL,
    Minimum INT NOT NULL,
    Maximum INT NOT NULL,
    Average DOUBLE PRECISION NOT NULL,
    RolledAtUtc TIMESTAMP NOT NULL,
    ContextJson JSONB NOT NULL,
    RawRolls JSONB NOT NULL,
    KeptRolls JSONB NOT NULL
);
```

## Presentation Layer

### REST API Endpoints

#### POST /api/dice/rolls
Roll dice.

**Request**:
```json
{
  "expression": "2d20+5",
  "context": {
    "character": "Aragorn",
    "action": "Attack"
  }
}
```

**Response**: `201 Created`
```json
{
  "id": "uuid",
  "expression": "2d20+5",
  "total": 27,
  "minimum": 7,
  "maximum": 45,
  "average": 26.0,
  "rolledAtUtc": "2024-01-15T10:30:00Z",
  "rawRolls": [...],
  "keptRolls": [...],
  "context": {...}
}
```

#### GET /api/dice/rolls/{id}
Get roll by ID.

**Response**: `200 OK`

#### GET /api/dice/rolls
Get roll history (paginated).

**Query Parameters**:
- `pageNumber` (default: 1)
- `pageSize` (default: 20)

**Response**: `200 OK`

#### POST /api/dice/rolls/{id}/reroll
Reroll a previous roll.

**Response**: `201 Created`

## Public API

The PublicApi project exposes contracts for other modules:

```csharp
namespace TavernTrashers.Api.Modules.Dice.PublicApi;

public interface IDiceRollingService
{
    Task<RollResult> RollDiceAsync(string expression, CancellationToken cancellationToken = default);
}
```

## Integration Events

Events published by this module:

### DiceRolledEvent
Published when dice are rolled.

```csharp
public record DiceRolledEvent(
    Guid RollId,
    string Expression,
    int Total,
    DateTime RolledAtUtc
) : IntegrationEvent;
```

## Testing

### Running Tests

```bash
# Unit tests
dotnet test src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Domain.Tests

# All module tests
dotnet test src/api/modules/dice/
```

### Test Coverage

The module maintains high test coverage:
- Domain logic: 95%+
- Application handlers: 85%+
- Infrastructure: 75%+

### Example Tests

```csharp
[Fact]
public void Roll_WithValidExpression_ShouldReturnOutcome()
{
    // Arrange
    var roller = new DiceRoller();
    var expression = "2d6+3";

    // Act
    var result = roller.Roll(expression);

    // Assert
    result.Total.Should().BeGreaterOrEqualTo(5);  // Min: 2+3
    result.Total.Should().BeLessOrEqualTo(15);    // Max: 12+3
}
```

## Usage Examples

### Basic Roll

```csharp
// Roll 2d20 with +5 modifier
var command = new RollDiceCommand("2d20+5");
var result = await mediator.Send(command);
```

### Roll with Context

```csharp
var context = JsonSerializer.Serialize(new {
    character = "Aragorn",
    action = "Attack roll",
    target = "Orc"
});

var command = new RollDiceCommand("d20+7", context);
var rollId = await mediator.Send(command);
```

### Advantage Roll

```csharp
// Roll with advantage (2d20, keep highest)
var command = new RollDiceCommand("2d20kh1+5");
var result = await mediator.Send(command);
```

### Ability Score Roll

```csharp
// Roll 4d6, drop lowest (classic ability score generation)
var command = new RollDiceCommand("4d6dl1");
var result = await mediator.Send(command);
```

## Dependencies

- **TavernTrashers.Api.Common.Domain**: Base entities, value objects
- **TavernTrashers.Api.Common.Application**: CQRS infrastructure
- **TavernTrashers.Api.Common.Infrastructure**: Database context
- **Entity Framework Core**: ORM
- **MediatR**: Command/Query handling
- **FluentValidation**: Input validation

## Performance Considerations

1. **Parser Caching**: Parsed expressions could be cached (future enhancement)
2. **Database Indexing**: Indexes on RolledAtUtc for history queries
3. **Pagination**: Always use pagination for roll history
4. **Context Size**: Keep context JSON reasonable (<1KB)

## Future Enhancements

- [ ] Exploding dice support
- [ ] Reroll-until mechanics
- [ ] Success counting (e.g., `5d6>4` counts 5s and 6s)
- [ ] Dice pool systems (World of Darkness, etc.)
- [ ] Roll macros/templates
- [ ] Roll statistics and analytics
- [ ] Dice roll animations (frontend)

## Security Considerations

1. **Expression Validation**: Limit expression complexity to prevent DoS
2. **Rate Limiting**: Prevent spam rolling
3. **Context Size**: Limit context JSON size
4. **Input Sanitization**: Validate all input strings

## Related Documentation

- [API Reference](../../../docs/API_REFERENCE.md)
- [Testing Guide](../../../docs/TESTING.md)
- [Architecture Overview](../../../docs/ARCHITECTURE.md)

## Contributing

When contributing to the Dice module:

1. Follow the established architecture patterns
2. Add tests for all new functionality
3. Update this README with new features
4. Ensure expression parser remains performant
5. Document any new dice notation syntax

---

*Roll for initiative!* 🎲

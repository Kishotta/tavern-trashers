# Characters Module

The Characters module manages character creation, character sheets, stats, classes, levels, and resources for tabletop RPG characters in Tavern Trashers.

## Overview

This module provides comprehensive character management functionality, including character creation, stat tracking, class/level management, and resource tracking (HP, spell slots, etc.).

## Features

- **Character Management**: Create, read, update, delete characters
- **Character Classes**: Define and manage character classes
- **Level Progression**: Track character levels and class levels
- **Resource Management**: Track HP, spell slots, and other resources
- **Stat Tracking**: Manage character attributes and derived stats
- **Multi-classing**: Support for multiple class levels per character

## Architecture

```
Characters Module
├── Domain/              # Core business logic
│   ├── Characters/      # Character entities
│   ├── Classes/         # Character class definitions
│   ├── ClassLevels/     # Level progression
│   └── Resources/       # Resource tracking (HP, spell slots)
├── Application/         # Use cases and handlers
│   ├── Characters/      # Character-related commands and queries
│   └── Classes/         # Class-related queries
├── Infrastructure/      # Data access
│   ├── Characters/      # Repository implementations
│   └── Classes/         # Class repository
└── Presentation/        # API endpoints
    └── Characters/      # REST API controllers
```

## Domain Layer

### Core Entities

#### Character
Represents a player character.

```csharp
public sealed class Character : Entity
{
    public string Name { get; }
    public Guid OwnerId { get; }                // User who owns this character
    public DateTime CreatedAtUtc { get; }
    public IReadOnlyCollection<ClassLevel> ClassLevels { get; }
    public IReadOnlyCollection<CharacterResource> Resources { get; }
    
    // Derived properties
    public int TotalLevel { get; }              // Sum of all class levels
    public CharacterClass PrimaryClass { get; }  // Highest level class
}
```

#### CharacterClass
Defines a character class (e.g., Fighter, Wizard, Rogue).

```csharp
public sealed class CharacterClass : Entity
{
    public string Name { get; }
    public string Description { get; }
    public int HitDie { get; }                  // e.g., 10 for Fighter (d10)
    public IReadOnlyCollection<ResourceDefinition> ResourceDefinitions { get; }
}
```

#### ClassLevel
Represents a level in a specific class.

```csharp
public sealed class ClassLevel : Entity
{
    public Guid CharacterId { get; }
    public Guid ClassId { get; }
    public int Level { get; }
    public DateTime AcquiredAtUtc { get; }
}
```

#### CharacterResource
Tracks character resources (HP, spell slots, etc.).

```csharp
public sealed class CharacterResource : Entity
{
    public Guid CharacterId { get; }
    public Guid ResourceDefinitionId { get; }
    public int CurrentValue { get; }
    public int MaximumValue { get; }
    
    public void Restore(int amount);
    public void Consume(int amount);
    public void RestoreToMaximum();
}
```

#### ResourceDefinition
Defines a type of resource (e.g., Hit Points, Spell Slots).

```csharp
public sealed class ResourceDefinition : Entity
{
    public string Name { get; }
    public string Description { get; }
    public ResourceType Type { get; }
    public bool RestoresOnShortRest { get; }
    public bool RestoresOnLongRest { get; }
}
```

### Domain Errors

```csharp
public static class CharacterErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Character.NotFound",
        "Character not found");
        
    public static readonly Error InvalidName = Error.Validation(
        "Character.InvalidName",
        "Character name must be between 1 and 100 characters");
        
    public static readonly Error InsufficientResources = Error.Failure(
        "Character.InsufficientResources",
        "Character does not have enough resources");
}
```

## Application Layer

### Commands

#### CreateCharacter
Create a new character.

```csharp
public record CreateCharacterCommand(
    string Name,
    Guid OwnerId,
    Guid ClassId,
    int StartingLevel = 1
) : ICommand<Guid>;
```

#### UpdateCharacter
Update character information.

```csharp
public record UpdateCharacterCommand(
    Guid CharacterId,
    string Name
) : ICommand;
```

#### DeleteCharacter
Delete a character.

```csharp
public record DeleteCharacterCommand(
    Guid CharacterId
) : ICommand;
```

#### AddClassLevel
Add a level in a class to a character.

```csharp
public record AddClassLevelCommand(
    Guid CharacterId,
    Guid ClassId
) : ICommand;
```

#### UpdateCharacterResource
Update a character's resource (e.g., take damage, use spell slot).

```csharp
public record UpdateCharacterResourceCommand(
    Guid CharacterId,
    Guid ResourceDefinitionId,
    int NewValue
) : ICommand;
```

#### RestCharacter
Rest and restore resources.

```csharp
public record RestCharacterCommand(
    Guid CharacterId,
    RestType RestType  // ShortRest or LongRest
) : ICommand;
```

### Queries

#### GetCharacter
Get character by ID.

```csharp
public record GetCharacterQuery(
    Guid CharacterId
) : IQuery<CharacterResponse>;
```

#### GetCharactersByOwner
Get all characters owned by a user.

```csharp
public record GetCharactersByOwnerQuery(
    Guid OwnerId,
    int PageNumber = 1,
    int PageSize = 20
) : IQuery<PagedResponse<CharacterSummaryResponse>>;
```

#### GetCharacterClasses
Get all available character classes.

```csharp
public record GetCharacterClassesQuery() 
    : IQuery<IReadOnlyCollection<CharacterClassResponse>>;
```

## Infrastructure Layer

### Repository

```csharp
public interface ICharacterRepository
{
    Task<Character?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Character>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task AddAsync(Character character, CancellationToken cancellationToken = default);
    Task UpdateAsync(Character character, CancellationToken cancellationToken = default);
    Task DeleteAsync(Character character, CancellationToken cancellationToken = default);
}

public interface ICharacterClassRepository
{
    Task<CharacterClass?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CharacterClass>> GetAllAsync(CancellationToken cancellationToken = default);
}
```

### Database Schema

```sql
CREATE TABLE Characters (
    Id UUID PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    OwnerId UUID NOT NULL,
    CreatedAtUtc TIMESTAMP NOT NULL,
    INDEX idx_owner (OwnerId)
);

CREATE TABLE CharacterClasses (
    Id UUID PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE,
    Description TEXT,
    HitDie INT NOT NULL
);

CREATE TABLE ClassLevels (
    Id UUID PRIMARY KEY,
    CharacterId UUID REFERENCES Characters(Id) ON DELETE CASCADE,
    ClassId UUID REFERENCES CharacterClasses(Id),
    Level INT NOT NULL,
    AcquiredAtUtc TIMESTAMP NOT NULL,
    INDEX idx_character (CharacterId)
);

CREATE TABLE ResourceDefinitions (
    Id UUID PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    Description TEXT,
    Type INT NOT NULL,
    RestoresOnShortRest BOOLEAN,
    RestoresOnLongRest BOOLEAN
);

CREATE TABLE CharacterResources (
    Id UUID PRIMARY KEY,
    CharacterId UUID REFERENCES Characters(Id) ON DELETE CASCADE,
    ResourceDefinitionId UUID REFERENCES ResourceDefinitions(Id),
    CurrentValue INT NOT NULL,
    MaximumValue INT NOT NULL,
    INDEX idx_character (CharacterId)
);
```

## Presentation Layer

### REST API Endpoints

#### POST /api/characters
Create a new character.

**Request**:
```json
{
  "name": "Aragorn",
  "classId": "fighter-class-id",
  "startingLevel": 1
}
```

**Response**: `201 Created`
```json
{
  "id": "character-id",
  "name": "Aragorn",
  "ownerId": "user-id",
  "totalLevel": 1,
  "classLevels": [
    {
      "classId": "fighter-class-id",
      "className": "Fighter",
      "level": 1
    }
  ],
  "resources": [
    {
      "name": "Hit Points",
      "current": 12,
      "maximum": 12
    }
  ],
  "createdAtUtc": "2024-01-15T10:00:00Z"
}
```

#### GET /api/characters/{id}
Get character details.

**Response**: `200 OK`

#### PUT /api/characters/{id}
Update character.

**Request**:
```json
{
  "name": "Aragorn, King of Gondor"
}
```

**Response**: `200 OK`

#### DELETE /api/characters/{id}
Delete character.

**Response**: `204 No Content`

#### GET /api/characters
Get user's characters (uses authenticated user's ID).

**Query Parameters**:
- `pageNumber` (default: 1)
- `pageSize` (default: 20)

**Response**: `200 OK`

#### POST /api/characters/{id}/levels
Add a class level to character.

**Request**:
```json
{
  "classId": "wizard-class-id"
}
```

**Response**: `200 OK`

#### PUT /api/characters/{id}/resources/{resourceId}
Update character resource.

**Request**:
```json
{
  "newValue": 8  // e.g., HP after taking damage
}
```

**Response**: `200 OK`

#### POST /api/characters/{id}/rest
Rest and restore resources.

**Request**:
```json
{
  "restType": "LongRest"  // or "ShortRest"
}
```

**Response**: `200 OK`

#### GET /api/characters/classes
Get all available character classes.

**Response**: `200 OK`
```json
{
  "classes": [
    {
      "id": "fighter-class-id",
      "name": "Fighter",
      "description": "A master of martial combat",
      "hitDie": 10
    },
    {
      "id": "wizard-class-id",
      "name": "Wizard",
      "description": "A scholarly magic-user",
      "hitDie": 6
    }
  ]
}
```

## Usage Examples

### Create a Character

```csharp
var command = new CreateCharacterCommand(
    "Gandalf",
    currentUserId,
    wizardClassId,
    startingLevel: 5
);

var characterId = await mediator.Send(command);
```

### Multi-class a Character

```csharp
// Add a level in Fighter to an existing Wizard
var command = new AddClassLevelCommand(
    characterId,
    fighterClassId
);

await mediator.Send(command);
```

### Track Damage

```csharp
var command = new UpdateCharacterResourceCommand(
    characterId,
    hitPointsResourceId,
    newValue: 8  // Character took damage
);

await mediator.Send(command);
```

### Long Rest

```csharp
var command = new RestCharacterCommand(
    characterId,
    RestType.LongRest
);

await mediator.Send(command);
// Character's HP and spell slots restored
```

## Resource Types

### Common Resources

1. **Hit Points**: Health/damage tracking
2. **Spell Slots**: Magic resource tracking
3. **Ki Points**: Monk class resource
4. **Rage Uses**: Barbarian class resource
5. **Sorcery Points**: Sorcerer meta-magic resource

### Resource Recovery

- **Short Rest**: Typically 1 hour, restores some resources
- **Long Rest**: Typically 8 hours, restores most resources

## Testing

### Running Tests

```bash
# Module tests (when available)
dotnet test src/api/modules/characters/
```

### Example Tests

```csharp
[Fact]
public void Create_WithValidName_ShouldSucceed()
{
    // Arrange
    var name = "Aragorn";
    var ownerId = Guid.NewGuid();
    var classId = Guid.NewGuid();

    // Act
    var result = Character.Create(name, ownerId, classId);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Name.Should().Be(name);
    result.Value.OwnerId.Should().Be(ownerId);
}

[Fact]
public void ConsumeResource_WithSufficientAmount_ShouldSucceed()
{
    // Arrange
    var resource = new CharacterResource(current: 10, maximum: 10);

    // Act
    var result = resource.Consume(5);

    // Assert
    result.IsSuccess.Should().BeTrue();
    resource.CurrentValue.Should().Be(5);
}
```

## Dependencies

- **TavernTrashers.Api.Common.Domain**: Base entities
- **TavernTrashers.Api.Common.Application**: CQRS infrastructure
- **TavernTrashers.Api.Common.Infrastructure**: Database context
- **Entity Framework Core**: ORM
- **MediatR**: Command/Query handling
- **FluentValidation**: Input validation

## Future Enhancements

- [ ] Character inventory system
- [ ] Equipment management
- [ ] Skill proficiencies
- [ ] Ability scores and modifiers
- [ ] Character backgrounds
- [ ] Feats and abilities
- [ ] Character portraits/avatars
- [ ] Character export (PDF, JSON)
- [ ] Character templates/archetypes
- [ ] Shared characters (party management)

## Integration with Other Modules

### With Users Module
- Characters are owned by users (OwnerId references User.Id)
- Permission checks for character operations

### With Dice Module
- Character context included in dice rolls
- Character stats affect roll modifiers

### With Campaigns Module (Future)
- Characters can be assigned to campaigns
- Campaign-specific character data

## Security Considerations

1. **Ownership Validation**: Ensure users can only modify their own characters
2. **Resource Validation**: Prevent invalid resource values
3. **Level Validation**: Ensure level progression rules are followed
4. **Input Sanitization**: Validate character names and descriptions

## Related Documentation

- [API Reference](../../../docs/API_REFERENCE.md)
- [Testing Guide](../../../docs/TESTING.md)
- [Architecture Overview](../../../docs/ARCHITECTURE.md)

## Contributing

When contributing to the Characters module:

1. Maintain clean separation between character and class concepts
2. Ensure resource tracking is accurate and consistent
3. Add tests for all character operations
4. Document any new character classes or resources
5. Consider multi-classing edge cases

---

*Build your hero!* ⚔️

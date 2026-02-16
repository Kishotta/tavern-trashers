# Common Domain Layer

The Common Domain layer provides shared domain building blocks and patterns used across all modules in Tavern Trashers.

## Overview

This library contains base classes, value objects, and domain primitives that are reused throughout the application to ensure consistency and reduce code duplication.

## Contents

```
TavernTrashers.Api.Common.Domain/
├── Entities/           # Base entity classes
│   ├── Entity.cs
│   ├── EntityBase.cs
│   ├── DomainEvent.cs
│   └── IDomainEvent.cs
├── Results/            # Result pattern implementation
│   ├── Result.cs
│   ├── Error.cs
│   └── ErrorType.cs
└── Auditing/           # Audit attributes
    ├── AuditableAttribute.cs
    └── NotAuditableAttribute.cs
```

## Core Components

### Entities

#### Entity<TId>
Base class for all domain entities with strongly-typed IDs.

```csharp
public abstract class Entity<TId> : EntityBase
{
    public TId Id { get; protected init; } = default!;
}
```

#### Entity
Convenience base class for entities with GUID IDs.

```csharp
public abstract class Entity : Entity<Guid>
{
    // Common entity with Guid ID
}
```

**Usage**:
```csharp
public sealed class Character : Entity
{
    // Character implementation
    // Automatically has Guid Id property
}
```

#### EntityBase
Abstract base providing domain event support.

```csharp
public abstract class EntityBase
{
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

**Key Features**:
- Domain event collection and management
- Protected methods to raise events
- Events cleared after processing

### Domain Events

#### IDomainEvent
Marker interface for domain events.

```csharp
public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOnUtc { get; }
}
```

#### DomainEvent
Base implementation of domain events.

```csharp
public abstract record DomainEvent(Guid Id, DateTime OccurredOnUtc) : IDomainEvent
{
    protected DomainEvent() 
        : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
```

**Usage**:
```csharp
public record CharacterCreatedEvent(
    Guid CharacterId,
    string CharacterName
) : DomainEvent;

// In entity
public static Result<Character> Create(string name)
{
    var character = new Character { Name = name };
    character.RaiseDomainEvent(new CharacterCreatedEvent(character.Id, name));
    return character;
}
```

### Result Pattern

#### Result<T>
Represents the outcome of an operation without exceptions.

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public Error Error { get; }
    
    public static Result<T> Success(T value);
    public static Result<T> Failure(Error error);
}

public class Result : Result<bool>
{
    // Result without value
}
```

**Usage**:
```csharp
// Returning success
public static Result<Character> Create(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        return Result<Character>.Failure(CharacterErrors.InvalidName);
        
    var character = new Character { Name = name };
    return Result<Character>.Success(character);
}

// Using result
var result = Character.Create("Aragorn");
if (result.IsFailure)
{
    // Handle error
    Console.WriteLine(result.Error.Message);
    return;
}

var character = result.Value;
```

#### Error
Represents an error with code, message, and type.

```csharp
public sealed record Error(
    string Code,
    string Message,
    ErrorType Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    
    public static Error Validation(string code, string message);
    public static Error NotFound(string code, string message);
    public static Error Conflict(string code, string message);
    public static Error Failure(string code, string message);
}
```

#### ErrorType
Categorizes errors.

```csharp
public enum ErrorType
{
    Validation,  // Input validation failed
    NotFound,    // Resource not found
    Conflict,    // Resource state conflict
    Failure      // General failure
}
```

**Usage**:
```csharp
public static class CharacterErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Character.NotFound",
        "The specified character was not found");
        
    public static readonly Error InvalidName = Error.Validation(
        "Character.InvalidName",
        "Character name must be between 1 and 100 characters");
        
    public static Error AlreadyExists(string name) => Error.Conflict(
        "Character.AlreadyExists",
        $"Character '{name}' already exists");
}
```

### Auditing

#### AuditableAttribute
Marks entities for audit tracking.

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class AuditableAttribute : Attribute
{
}
```

#### NotAuditableAttribute
Marks entities to exclude from auditing.

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class NotAuditableAttribute : Attribute
{
}
```

**Usage**:
```csharp
[Auditable]
public sealed class Character : Entity
{
    // This entity will be audited
}

[NotAuditable]
public sealed class AuditLog : Entity
{
    // Don't audit the audit logs themselves
}
```

## Design Patterns

### Entity Equality

Entities are equal if their IDs are equal:

```csharp
var character1 = new Character { Id = guid };
var character2 = new Character { Id = guid };

character1.Equals(character2); // True - same ID
```

### Domain Event Publishing

1. **Raise Event in Entity**:
```csharp
public static Result<Character> Create(string name)
{
    var character = new Character { Name = name };
    character.RaiseDomainEvent(new CharacterCreatedEvent(character.Id, name));
    return character;
}
```

2. **Events Collected**:
```csharp
var events = character.DomainEvents;
// Events are collected but not yet published
```

3. **Events Published**:
```csharp
// In infrastructure layer after SaveChanges
foreach (var domainEvent in entity.DomainEvents)
{
    await publisher.PublishAsync(domainEvent);
}
entity.ClearDomainEvents();
```

### Result Pattern Benefits

1. **No Exceptions for Expected Errors**: Better performance
2. **Explicit Error Handling**: Forces consumers to handle errors
3. **Type Safety**: Compile-time error checking
4. **Functional Approach**: Railway-oriented programming

## Best Practices

### Entity Design

✅ **Do**:
- Use private setters for properties
- Validate in factory methods
- Raise domain events for important changes
- Keep entities focused (Single Responsibility)

❌ **Don't**:
- Allow invalid entity states
- Use public setters
- Throw exceptions for business rule violations
- Create anemic entities (just getters/setters)

### Result Pattern

✅ **Do**:
```csharp
// Return Result for expected failures
public static Result<Character> Create(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        return Result<Character>.Failure(CharacterErrors.InvalidName);
    // ...
}
```

❌ **Don't**:
```csharp
// Don't throw for business rule violations
public static Character Create(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Invalid name"); // ❌
    // ...
}
```

### Domain Events

✅ **Do**:
- Raise events for significant domain changes
- Use past tense for event names
- Include relevant data in events

❌ **Don't**:
- Raise events for trivial changes
- Include entire entity in events
- Create circular event dependencies

## Testing

### Testing Entities

```csharp
[Fact]
public void Create_WithValidName_ShouldSucceed()
{
    // Arrange
    var name = "Aragorn";

    // Act
    var result = Character.Create(name);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Name.Should().Be(name);
}

[Fact]
public void Create_WithEmptyName_ShouldReturnValidationError()
{
    // Act
    var result = Character.Create(string.Empty);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Type.Should().Be(ErrorType.Validation);
}
```

### Testing Domain Events

```csharp
[Fact]
public void Create_ShouldRaiseDomainEvent()
{
    // Act
    var result = Character.Create("Gandalf");

    // Assert
    result.Value.DomainEvents.Should().ContainSingle();
    result.Value.DomainEvents.Should().ContainSingle(e => 
        e is CharacterCreatedEvent);
}
```

## Dependencies

This is a core library with minimal dependencies:
- No external NuGet packages
- No dependency on other application layers
- Used by all other layers

## Related Documentation

- [Architecture Overview](../../../../docs/ARCHITECTURE.md)
- [Development Guide](../../../../docs/DEVELOPMENT.md)
- [Testing Guide](../../../../docs/TESTING.md)

## Contributing

When contributing to the Common Domain layer:

1. Keep it framework-agnostic (no EF Core, no ASP.NET)
2. Maintain high test coverage (95%+)
3. Document all public APIs
4. Consider backward compatibility
5. Discuss breaking changes with team

---

*The foundation of domain-driven design!* 🏗️

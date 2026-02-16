# Coding Conventions and Standards

This document outlines the coding conventions, standards, and best practices for the Tavern Trashers codebase.

## Table of Contents

- [General Principles](#general-principles)
- [C# Conventions](#c-conventions)
- [TypeScript/Angular Conventions](#typescriptangular-conventions)
- [Architecture Patterns](#architecture-patterns)
- [Naming Conventions](#naming-conventions)
- [Code Organization](#code-organization)
- [Best Practices](#best-practices)

## General Principles

### SOLID Principles

1. **Single Responsibility**: Each class should have one reason to change
2. **Open/Closed**: Open for extension, closed for modification
3. **Liskov Substitution**: Subtypes must be substitutable for base types
4. **Interface Segregation**: Many specific interfaces better than one general
5. **Dependency Inversion**: Depend on abstractions, not concretions

### Clean Code

- **Meaningful Names**: Use descriptive, intention-revealing names
- **Small Functions**: Functions should do one thing well
- **Comments**: Code should be self-documenting; use comments for "why," not "what"
- **Error Handling**: Use exceptions for exceptional cases, Result pattern for expected failures
- **Testing**: Write tests first or immediately after implementation

## C# Conventions

### Formatting

Follow [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

```csharp
// ✅ Good
public class Character : Entity
{
    public string Name { get; private set; }
    
    public static Result<Character> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CharacterErrors.InvalidName;
            
        return new Character { Name = name };
    }
}

// ❌ Bad
public class character
{
    public string name;
    public character(string n) { name = n; }
}
```

### Naming

- **Classes**: PascalCase (e.g., `CharacterRepository`)
- **Methods**: PascalCase (e.g., `GetCharacterAsync`)
- **Properties**: PascalCase (e.g., `FirstName`)
- **Private Fields**: _camelCase (e.g., `_repository`)
- **Parameters**: camelCase (e.g., `userId`)
- **Local Variables**: camelCase (e.g., `character`)
- **Constants**: PascalCase (e.g., `MaxCharacterNameLength`)
- **Interfaces**: IPascalCase (e.g., `ICharacterRepository`)

### Async/Await

Always use async/await for I/O operations:

```csharp
// ✅ Good
public async Task<Result<Character>> GetCharacterAsync(Guid id, CancellationToken ct)
{
    var character = await _repository.GetByIdAsync(id, ct);
    return character ?? CharacterErrors.NotFound;
}

// ❌ Bad
public Character GetCharacter(Guid id)
{
    return _repository.GetById(id);  // Blocks thread
}
```

### Result Pattern

Use Result pattern instead of exceptions for expected failures:

```csharp
// ✅ Good
public static Result<Character> Create(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        return CharacterErrors.InvalidName;
        
    return new Character { Name = name };
}

// ❌ Bad
public static Character Create(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Invalid name");  // Don't throw for business rules
        
    return new Character { Name = name };
}
```

### Entity Design

```csharp
// ✅ Good - Rich domain model
public sealed class Character : Entity
{
    private Character() { }  // EF Core constructor
    
    public string Name { get; private set; }
    public int Level { get; private set; }
    
    public static Result<Character> Create(string name)
    {
        // Validation logic
        return new Character { Name = name, Level = 1 };
    }
    
    public Result LevelUp()
    {
        if (Level >= 20)
            return CharacterErrors.MaxLevelReached;
            
        Level++;
        RaiseDomainEvent(new CharacterLeveledUpEvent(Id, Level));
        return Result.Success();
    }
}

// ❌ Bad - Anemic model
public class Character
{
    public string Name { get; set; }  // Public setters
    public int Level { get; set; }
}
```

### CQRS

Separate commands and queries:

```csharp
// Command - changes state
public record CreateCharacterCommand(
    string Name,
    Guid OwnerId
) : ICommand<Guid>;

// Query - reads data
public record GetCharacterQuery(Guid CharacterId) : IQuery<CharacterResponse>;
```

## TypeScript/Angular Conventions

### Formatting

Follow [Angular Style Guide](https://angular.dev/style-guide).

### Component Structure

```typescript
// ✅ Good
@Component({
  selector: 'app-character-list',
  standalone: true,
  templateUrl: './character-list.component.html',
  styleUrls: ['./character-list.component.scss']
})
export class CharacterListComponent implements OnInit {
  characters = signal<Character[]>([]);
  
  constructor(private characterService: CharacterService) {}
  
  ngOnInit(): void {
    this.loadCharacters();
  }
  
  private loadCharacters(): void {
    this.characterService.getCharacters()
      .subscribe(chars => this.characters.set(chars));
  }
}
```

### Naming

- **Components**: PascalCase with `.component.ts` suffix
- **Services**: PascalCase with `.service.ts` suffix
- **Interfaces**: PascalCase (no 'I' prefix in TypeScript)
- **Variables**: camelCase
- **Constants**: UPPER_SNAKE_CASE
- **Observables**: Suffix with `$` (e.g., `character$`)

### RxJS

```typescript
// ✅ Good - Use operators, unsubscribe properly
export class CharacterComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  ngOnInit(): void {
    this.characterService.getCharacter(this.id)
      .pipe(
        takeUntil(this.destroy$),
        catchError(error => {
          console.error('Error loading character', error);
          return of(null);
        })
      )
      .subscribe(character => this.character.set(character));
  }
  
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

// Or use async pipe in template
character$ = this.characterService.getCharacter(this.id);
```

## Architecture Patterns

### Clean Architecture Layers

```
Presentation Layer (Controllers, API Endpoints)
        ↓
Application Layer (Commands, Queries, Handlers)
        ↓
Domain Layer (Entities, Value Objects, Domain Events)
        ↑
Infrastructure Layer (Repositories, External Services)
```

**Dependency Rule**: Inner layers don't depend on outer layers.

### Repository Pattern

```csharp
// Interface in Domain layer
public interface ICharacterRepository
{
    Task<Character?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Character character, CancellationToken ct = default);
}

// Implementation in Infrastructure layer
public class CharacterRepository : ICharacterRepository
{
    private readonly CharactersDbContext _context;
    
    public async Task<Character?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Characters.FindAsync(new object[] { id }, ct);
    }
}
```

### Unit of Work Pattern

Automatically managed by EF Core DbContext.

## Naming Conventions

### Commands and Queries

- Commands: **Verb + Noun** (e.g., `CreateCharacter`, `UpdateCharacter`)
- Queries: **Get + Noun** (e.g., `GetCharacter`, `GetCharacters`)

### Domain Events

- Past tense: **Noun + Past Verb** (e.g., `CharacterCreated`, `DiceRolled`)

### Integration Events

- Past tense with "IntegrationEvent" suffix (e.g., `CharacterCreatedIntegrationEvent`)

### Error Types

- Static properties on Error class: `CharacterErrors.NotFound`

## Code Organization

### Module Structure

```
TavernTrashers.Api.Modules.Characters/
├── Domain/
│   ├── Characters/
│   │   ├── Character.cs
│   │   ├── ICharacterRepository.cs
│   │   └── CharacterErrors.cs
│   └── Classes/
│       └── CharacterClass.cs
├── Application/
│   ├── Characters/
│   │   ├── CreateCharacter.cs
│   │   └── GetCharacter.cs
│   └── IntegrationEvents/
│       └── CharacterCreatedEvent.cs
├── Infrastructure/
│   ├── Characters/
│   │   ├── CharacterRepository.cs
│   │   └── CharacterConfiguration.cs
│   └── CharactersDbContext.cs
└── Presentation/
    └── Characters/
        ├── CreateCharacter.cs
        └── GetCharacter.cs
```

### File Organization

One class per file, file name matches class name.

## Best Practices

### Validation

```csharp
// ✅ Good - FluentValidation
public class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand>
{
    public CreateCharacterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
            
        RuleFor(x => x.OwnerId)
            .NotEmpty();
    }
}
```

### Dependency Injection

```csharp
// ✅ Good - Constructor injection
public class CreateCharacterHandler : ICommandHandler<CreateCharacterCommand, Guid>
{
    private readonly ICharacterRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateCharacterHandler(
        ICharacterRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
}

// ❌ Bad - Service locator
public class CreateCharacterHandler
{
    private readonly IServiceProvider _serviceProvider;
    
    public void Handle()
    {
        var repository = _serviceProvider.GetService<ICharacterRepository>();
    }
}
```

### Testing

```csharp
// ✅ Good - AAA pattern
[Fact]
public async Task CreateCharacter_WithValidName_ShouldSucceed()
{
    // Arrange
    var command = new CreateCharacterCommand("Aragorn", ownerId, classId);
    var mockRepo = new Mock<ICharacterRepository>();
    var handler = new CreateCharacterHandler(mockRepo.Object);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    mockRepo.Verify(r => r.AddAsync(It.IsAny<Character>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

### Logging

```csharp
// ✅ Good - Structured logging
_logger.LogInformation(
    "Character {CharacterId} created by user {UserId}",
    character.Id,
    userId);

// ❌ Bad - String concatenation
_logger.LogInformation($"Character {character.Id} created by user {userId}");
```

### Error Messages

- User-facing: Clear, actionable messages
- Logs: Include technical details, stack traces
- Never expose sensitive information

## Documentation

### XML Documentation

```csharp
/// <summary>
/// Creates a new character with the specified name and owner.
/// </summary>
/// <param name="name">The character's name.</param>
/// <param name="ownerId">The ID of the user who owns this character.</param>
/// <returns>A result containing the character ID if successful, or an error if validation fails.</returns>
public static Result<Character> Create(string name, Guid ownerId)
{
    // Implementation
}
```

### Code Comments

```csharp
// ✅ Good - Explain why
// We use a separate transaction here to ensure character creation
// and initial equipment assignment are atomic
await using var transaction = await _unitOfWork.BeginTransactionAsync();

// ❌ Bad - Explain what (code should be self-explanatory)
// Set the name
character.Name = name;
```

## Security

1. **Never log sensitive data** (passwords, tokens, personal info)
2. **Validate all input** (use FluentValidation)
3. **Sanitize output** (prevent XSS)
4. **Use parameterized queries** (prevent SQL injection)
5. **Implement proper authorization** (check permissions)

## Performance

1. **Use async/await** for I/O operations
2. **Avoid N+1 queries** (use Include/eager loading)
3. **Use caching** for frequently accessed data
4. **Implement pagination** for large result sets
5. **Profile before optimizing** (don't guess)

## Related Documentation

- [Development Guide](./DEVELOPMENT.md)
- [Testing Guide](./TESTING.md)
- [Architecture Overview](./ARCHITECTURE.md)

---

*Quality code is a team effort!* 💻

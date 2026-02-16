# Common Application Layer

The Common Application layer provides shared application patterns, CQRS infrastructure, behaviors, and cross-cutting concerns used across all modules.

## Overview

This library implements the application layer patterns following Clean Architecture principles, including Command Query Responsibility Segregation (CQRS), MediatR pipelines, caching, and authorization.

## Contents

```
TavernTrashers.Api.Common.Application/
├── Messaging/           # CQRS infrastructure
│   ├── ICommand.cs
│   ├── ICommandHandler.cs
│   ├── IQuery.cs
│   ├── IQueryHandler.cs
│   ├── IDomainEventHandler.cs
│   └── DomainEventHandler.cs
├── Behaviors/           # MediatR pipeline behaviors
│   ├── ValidationBehavior.cs
│   ├── LoggingBehavior.cs
│   └── UnitOfWorkBehavior.cs
├── Caching/             # Caching abstractions
│   └── ICacheService.cs
├── Authorization/       # Permission checking
│   ├── IPermissionService.cs
│   └── PermissionResponse.cs
├── Authentication/      # User context
│   └── IUserContext.cs
├── Data/                # Data access patterns
│   ├── IUnitOfWork.cs
│   ├── IDbConnectionFactory.cs
│   └── IUnitOfWorkExtensions.cs
├── EventBus/            # Integration event bus
│   ├── IEventBus.cs
│   └── IntegrationEvent.cs
├── Clock/               # Time abstraction
│   └── IDateTimeProvider.cs
├── Exceptions/          # Application exceptions
│   └── TavernTrashersException.cs
└── Modules/             # Module registry
    └── IModule.cs
```

## CQRS Infrastructure

### Commands

Commands represent write operations that change system state.

#### ICommand
```csharp
public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
```

#### ICommandHandler
```csharp
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}

public interface ICommandHandler<in TCommand, TResponse> 
    : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}
```

**Usage**:
```csharp
// Define command
public record CreateCharacterCommand(
    string Name,
    Guid OwnerId,
    Guid ClassId
) : ICommand<Guid>;

// Implement handler
public class CreateCharacterHandler : ICommandHandler<CreateCharacterCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateCharacterCommand command, 
        CancellationToken cancellationToken)
    {
        var character = Character.Create(command.Name, command.OwnerId, command.ClassId);
        if (character.IsFailure)
            return Result<Guid>.Failure(character.Error);
            
        await _repository.AddAsync(character.Value, cancellationToken);
        
        return Result<Guid>.Success(character.Value.Id);
    }
}
```

### Queries

Queries represent read operations that don't change state.

#### IQuery
```csharp
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
```

#### IQueryHandler
```csharp
public interface IQueryHandler<in TQuery, TResponse> 
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
```

**Usage**:
```csharp
// Define query
public record GetCharacterQuery(Guid CharacterId) : IQuery<CharacterResponse>;

// Implement handler
public class GetCharacterQueryHandler : IQueryHandler<GetCharacterQuery, CharacterResponse>
{
    public async Task<Result<CharacterResponse>> Handle(
        GetCharacterQuery query, 
        CancellationToken cancellationToken)
    {
        var character = await _repository.GetByIdAsync(query.CharacterId, cancellationToken);
        
        if (character is null)
            return Result<CharacterResponse>.Failure(CharacterErrors.NotFound);
            
        var response = new CharacterResponse(character.Id, character.Name);
        return Result<CharacterResponse>.Success(response);
    }
}
```

### Domain Event Handlers

Handle domain events raised by entities.

#### IDomainEventHandler
```csharp
public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
}
```

**Usage**:
```csharp
public class CharacterCreatedEventHandler : IDomainEventHandler<CharacterCreatedEvent>
{
    public async Task Handle(
        CharacterCreatedEvent domainEvent, 
        CancellationToken cancellationToken)
    {
        // Send welcome email
        // Update statistics
        // Publish integration event
        await _eventBus.PublishAsync(
            new CharacterCreatedIntegrationEvent(domainEvent.CharacterId),
            cancellationToken);
    }
}
```

## Pipeline Behaviors

MediatR pipeline behaviors provide cross-cutting concerns.

### Validation Behavior

Validates commands/queries using FluentValidation.

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            // Return validation errors
        }
        
        return await next();
    }
}
```

### Logging Behavior

Logs all commands and queries.

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
        
        var response = await next();
        
        _logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);
        
        return response;
    }
}
```

### Unit of Work Behavior

Manages database transactions.

```csharp
public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!typeof(TRequest).IsCommand())
        {
            return await next();
        }
        
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        var response = await next();
        
        await transaction.CommitAsync(cancellationToken);
        
        return response;
    }
}
```

**Pipeline Order**:
1. Logging (before)
2. Validation
3. Unit of Work
4. Handler execution
5. Logging (after)

## Caching

### ICacheService

Abstract caching interface.

```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    
    Task SetAsync<T>(
        string key, 
        T value, 
        TimeSpan? expiration = null, 
        CancellationToken cancellationToken = default);
        
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}
```

**Usage**:
```csharp
public class GetCharacterQueryHandler
{
    public async Task<Result<CharacterResponse>> Handle(
        GetCharacterQuery query,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"character:{query.CharacterId}";
        
        // Try cache first
        var cached = await _cache.GetAsync<CharacterResponse>(cacheKey, cancellationToken);
        if (cached is not null)
            return Result<CharacterResponse>.Success(cached);
            
        // Get from database
        var character = await _repository.GetByIdAsync(query.CharacterId, cancellationToken);
        // ...
        
        // Cache the result
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken);
        
        return Result<CharacterResponse>.Success(response);
    }
}
```

## Authorization

### IPermissionService

Check user permissions.

```csharp
public interface IPermissionService
{
    Task<PermissionResponse> CheckPermissionAsync(
        string permission,
        CancellationToken cancellationToken = default);
}
```

**Usage**:
```csharp
public class DeleteCharacterHandler
{
    public async Task<Result> Handle(
        DeleteCharacterCommand command,
        CancellationToken cancellationToken)
    {
        var permissionResponse = await _permissionService.CheckPermissionAsync(
            "delete:characters",
            cancellationToken);
            
        if (!permissionResponse.IsPermitted)
            return Result.Failure(CharacterErrors.InsufficientPermissions);
            
        // Process deletion
    }
}
```

## Authentication

### IUserContext

Access current user information.

```csharp
public interface IUserContext
{
    Guid UserId { get; }
    string IdentityId { get; }
    bool IsAuthenticated { get; }
}
```

**Usage**:
```csharp
public class CreateCharacterHandler
{
    public async Task<Result<Guid>> Handle(
        CreateCharacterCommand command,
        CancellationToken cancellationToken)
    {
        // Use current user as owner
        var character = Character.Create(
            command.Name,
            _userContext.UserId,
            command.ClassId);
            
        // ...
    }
}
```

## Data Access

### IUnitOfWork

Manages database transactions and change tracking.

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

### IDbConnectionFactory

Creates database connections for queries.

```csharp
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
```

## Event Bus

### IEventBus

Publish integration events to other modules.

```csharp
public interface IEventBus
{
    Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : IntegrationEvent;
}
```

### IntegrationEvent

Base class for integration events.

```csharp
public abstract record IntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
```

**Usage**:
```csharp
// Define integration event
public record CharacterCreatedIntegrationEvent(
    Guid CharacterId,
    string CharacterName,
    Guid OwnerId
) : IntegrationEvent;

// Publish event
await _eventBus.PublishAsync(
    new CharacterCreatedIntegrationEvent(character.Id, character.Name, character.OwnerId),
    cancellationToken);
```

## Time Abstraction

### IDateTimeProvider

Testable time provider.

```csharp
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}
```

**Benefits**:
- Testable time-dependent logic
- Consistent timezone handling
- Easy to mock in tests

## Module System

### IModule

Module registration interface.

```csharp
public interface IModule
{
    void RegisterServices(IServiceCollection services, IConfiguration configuration);
    void RegisterEndpoints(IEndpointRouteBuilder app);
}
```

**Usage**:
```csharp
public class DiceModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDiceRoller, DiceRoller>();
        services.AddScoped<IRollRepository, RollRepository>();
    }
    
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        // Minimal API endpoints
    }
}
```

## Best Practices

### Command/Query Separation

✅ **Do**:
- Commands for write operations
- Queries for read operations
- Keep them separate

❌ **Don't**:
- Return data from commands (return just ID)
- Modify state in queries

### Handler Responsibility

✅ **Do**:
- Single responsibility per handler
- Thin handlers (orchestrate, don't implement)
- Delegate to domain for business logic

❌ **Don't**:
- Put business logic in handlers
- Make handlers fat
- Access multiple aggregates directly

### Validation

✅ **Do**:
```csharp
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

## Testing

### Testing Command Handlers

```csharp
[Fact]
public async Task Handle_WithValidCommand_ShouldCreateCharacter()
{
    // Arrange
    var command = new CreateCharacterCommand("Aragorn", ownerId, classId);
    var mockRepository = new Mock<ICharacterRepository>();
    var handler = new CreateCharacterHandler(mockRepository.Object);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    mockRepository.Verify(r => 
        r.AddAsync(It.IsAny<Character>(), It.IsAny<CancellationToken>()), 
        Times.Once);
}
```

### Testing Query Handlers

```csharp
[Fact]
public async Task Handle_WithExistingCharacter_ShouldReturnCharacter()
{
    // Arrange
    var query = new GetCharacterQuery(characterId);
    var character = new Character { Id = characterId, Name = "Gandalf" };
    var mockRepository = new Mock<ICharacterRepository>();
    mockRepository.Setup(r => r.GetByIdAsync(characterId, default))
        .ReturnsAsync(character);
    
    var handler = new GetCharacterQueryHandler(mockRepository.Object);
    
    // Act
    var result = await handler.Handle(query, CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Name.Should().Be("Gandalf");
}
```

## Dependencies

- **MediatR**: CQRS infrastructure
- **FluentValidation**: Command/query validation
- **TavernTrashers.Api.Common.Domain**: Domain primitives

## Related Documentation

- [Common Domain Layer](../TavernTrashers.Api.Common.Domain/README.md)
- [Architecture Overview](../../../../docs/ARCHITECTURE.md)
- [Testing Guide](../../../../docs/TESTING.md)

## Contributing

When contributing to the Common Application layer:

1. Follow CQRS patterns consistently
2. Keep behaviors generic and reusable
3. Document pipeline order for new behaviors
4. Test all behaviors thoroughly
5. Consider performance impact

---

*Application logic done right!* ⚙️

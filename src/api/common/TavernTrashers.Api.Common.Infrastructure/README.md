# Common Infrastructure Layer

The Common Infrastructure layer provides shared infrastructure implementations including database access, caching, messaging, authentication, and external service integrations.

## Overview

This library contains concrete implementations of infrastructure concerns that are shared across all modules, following the Dependency Inversion Principle.

## Contents

```
TavernTrashers.Api.Common.Infrastructure/
├── Database/           # EF Core and database setup
├── Caching/            # Redis caching implementation
├── EventBus/           # RabbitMQ message broker
├── Outbox/             # Transactional outbox pattern
├── Inbox/              # Idempotent inbox pattern  
├── Authentication/     # JWT and Keycloak integration
├── Authorization/      # Permission service implementation
├── Clock/              # DateTime provider
├── Auditing/           # Audit logging
├── Serialization/      # JSON serialization
└── Modules/            # Module loading infrastructure
```

## Key Components

### Database

**DbContext Base**: Shared database context configuration
```csharp
public abstract class ModuleDbContext : DbContext
{
    protected ModuleDbContext(DbContextOptions options) : base(options) { }
    
    // Configured with:
    // - DbContextPool for performance
    // - Automatic audit fields
    // - Domain event publishing
    // - Optimistic concurrency
}
```

**Unit of Work**: Transaction management
```csharp
public class UnitOfWork : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        // Publishes domain events
        // Updates audit fields
        // Saves changes atomically
    }
}
```

### Caching

**Redis Cache Service**: Distributed caching
```csharp
public class RedisCacheService : ICacheService
{
    // Implements caching with Redis
    // JSON serialization
    // Configurable expiration
    // Key prefix support
}
```

### Event Bus

**RabbitMQ Event Bus**: Message broker integration
```csharp
public class EventBus : IEventBus
{
    public async Task PublishAsync<T>(T integrationEvent, CancellationToken ct)
    {
        // Serializes event
        // Publishes to RabbitMQ
        // Ensures delivery
    }
}
```

### Outbox Pattern

**Transactional Outbox**: Ensures reliable event publishing
```csharp
// Events saved to outbox table in same transaction
// Background job processes outbox
// Guarantees at-least-once delivery
```

**Configuration**:
```csharp
services.AddQuartz(q =>
{
    q.ScheduleJob<ProcessOutboxJob>(trigger => trigger
        .WithSimpleSchedule(s => s
            .WithIntervalInSeconds(10)
            .RepeatForever()));
});
```

### Inbox Pattern

**Idempotent Inbox**: Prevents duplicate event processing
```csharp
// Events checked against inbox
// Duplicate events ignored
// Ensures exactly-once semantics
```

### Authentication

**JWT Authentication**: Token validation
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakSettings.Authority;
        options.Audience = keycloakSettings.Audience;
    });
```

### Authorization

**Permission Service**: Fine-grained permissions
```csharp
public class PermissionService : IPermissionService
{
    // Checks user permissions
    // Caches permission sets
    // Integrates with Keycloak
}
```

## Configuration

### Database Connection

```json
{
  "ConnectionStrings": {
    "Database": "Server=localhost;Port=5432;Database=taverntrashers;User Id=postgres;Password=postgres;"
  }
}
```

### Redis

```json
{
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "TavernTrashers:"
  }
}
```

### RabbitMQ

```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  }
}
```

### Keycloak

```json
{
  "Keycloak": {
    "Authority": "http://localhost:8080/realms/tavern-trashers",
    "Audience": "tavern-trashers-api",
    "RequireHttpsMetadata": false
  }
}
```

## Usage Examples

### Registering Infrastructure

```csharp
builder.Services.AddInfrastructure(builder.Configuration);
```

### Using DbContext

```csharp
public class RollRepository : IRollRepository
{
    private readonly DiceDbContext _context;
    
    public async Task AddAsync(Roll roll, CancellationToken ct)
    {
        await _context.Rolls.AddAsync(roll, ct);
    }
}
```

### Publishing Events

```csharp
await _eventBus.PublishAsync(
    new CharacterCreatedIntegrationEvent(character.Id),
    cancellationToken);
```

## Performance Features

1. **DbContext Pooling**: Reuses context instances
2. **Connection Pooling**: Efficient database connections
3. **Redis Caching**: Fast distributed cache
4. **Async/Await**: Non-blocking I/O
5. **Compiled Queries**: EF Core query compilation

## Dependencies

- **Entity Framework Core**: ORM
- **Npgsql**: PostgreSQL provider
- **StackExchange.Redis**: Redis client
- **RabbitMQ.Client**: RabbitMQ client
- **Quartz.NET**: Job scheduling
- **Serilog**: Logging

## Related Documentation

- [Common Application Layer](../TavernTrashers.Api.Common.Application/README.md)
- [Development Guide](../../../../docs/DEVELOPMENT.md)

---

*Infrastructure that scales!* 🏗️

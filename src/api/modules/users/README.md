# Users Module

The Users module manages user accounts, authentication, authorization, and permissions within Tavern Trashers. It integrates with Keycloak for identity management and provides a comprehensive user management system.

## Overview

This module serves as the authentication and authorization hub for the entire application, handling user registration, profile management, role-based access control (RBAC), and integration with the Keycloak identity provider.

## Features

- **User Management**: Create, read, update user profiles
- **Keycloak Integration**: OAuth 2.0 / OpenID Connect authentication
- **Role-Based Access Control**: Manage user roles and permissions
- **User Registration**: New user account creation
- **Profile Management**: User profile updates and preferences
- **Integration Events**: Publish user-related events to other modules
- **Permission System**: Fine-grained permission checking

## Architecture

```
Users Module
├── Domain/              # Core business logic
│   ├── Users/           # User entities and value objects
│   ├── Roles/           # Role and permission definitions
│   └── Permissions/     # Permission-related logic
├── Application/         # Use cases and handlers
│   ├── Users/           # User-related commands and queries
│   └── Authorization/   # Authorization logic
├── Infrastructure/      # External integrations
│   ├── Identity/        # Keycloak integration
│   ├── Authorization/   # Permission services
│   ├── Users/           # Repository implementations
│   └── Outbox/          # Outbox pattern for events
├── Presentation/        # API endpoints
│   └── Users/           # REST API controllers
└── IntegrationEvents/   # Events published to other modules
```

## Domain Layer

### Core Entities

#### User
Represents a user in the system.

```csharp
public sealed class User : Entity
{
    public string IdentityId { get; }      // Keycloak user ID
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string DisplayName { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? LastLoginAtUtc { get; }
    public IReadOnlyCollection<Role> Roles { get; }
}
```

#### Role
Represents a user role.

```csharp
public sealed class Role : Enumeration
{
    public static readonly Role Player = new(1, "Player");
    public static readonly Role GameMaster = new(2, "GameMaster");
    public static readonly Role Administrator = new(3, "Administrator");
    
    public IReadOnlyCollection<Permission> Permissions { get; }
}
```

#### Permission
Represents a specific permission.

```csharp
public sealed class Permission : Enumeration
{
    public static readonly Permission ReadCharacters = new(1, "read:characters");
    public static readonly Permission WriteCharacters = new(2, "write:characters");
    public static readonly Permission ManageUsers = new(3, "manage:users");
    // ... more permissions
}
```

## Application Layer

### Commands

#### RegisterUser
Register a new user.

```csharp
public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : ICommand<Guid>;
```

#### UpdateUserProfile
Update user profile information.

```csharp
public record UpdateUserProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string DisplayName
) : ICommand;
```

### Queries

#### GetUser
Get user by ID.

```csharp
public record GetUserQuery(
    Guid UserId
) : IQuery<UserResponse>;
```

#### GetCurrentUser
Get the currently authenticated user.

```csharp
public record GetCurrentUserQuery() : IQuery<UserResponse>;
```

#### GetUsers
Get paginated list of users.

```csharp
public record GetUsersQuery(
    int PageNumber = 1,
    int PageSize = 20
) : IQuery<PagedResponse<UserResponse>>;
```

### Authorization

#### Permission Checking

```csharp
public interface IPermissionService
{
    Task<bool> HasPermissionAsync(
        string identityId, 
        string permission, 
        CancellationToken cancellationToken = default);
        
    Task<HashSet<string>> GetPermissionsAsync(
        string identityId, 
        CancellationToken cancellationToken = default);
}
```

## Infrastructure Layer

### Keycloak Integration

The module integrates with Keycloak for identity management:

```csharp
public interface IIdentityProviderService
{
    Task<string> RegisterUserAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);
        
    Task<UserRepresentation> GetUserAsync(
        string identityId,
        CancellationToken cancellationToken = default);
        
    Task UpdateUserAsync(
        string identityId,
        UserUpdateRequest request,
        CancellationToken cancellationToken = default);
}
```

### Repository

```csharp
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdentityIdAsync(string identityId, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}
```

### Database Schema

```sql
CREATE TABLE Users (
    Id UUID PRIMARY KEY,
    IdentityId VARCHAR(255) UNIQUE NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    DisplayName VARCHAR(200),
    CreatedAtUtc TIMESTAMP NOT NULL,
    LastLoginAtUtc TIMESTAMP,
    INDEX idx_identity_id (IdentityId),
    INDEX idx_email (Email)
);

CREATE TABLE UserRoles (
    UserId UUID REFERENCES Users(Id),
    RoleId INT,
    PRIMARY KEY (UserId, RoleId)
);
```

## Presentation Layer

### REST API Endpoints

#### POST /api/users/register
Register a new user.

**Request**:
```json
{
  "email": "player@example.com",
  "password": "SecurePassword123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response**: `201 Created`
```json
{
  "id": "user-id",
  "email": "player@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "displayName": "John Doe",
  "createdAtUtc": "2024-01-15T10:00:00Z"
}
```

#### GET /api/users/me
Get current authenticated user.

**Response**: `200 OK`
```json
{
  "id": "user-id",
  "email": "player@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "displayName": "John Doe",
  "roles": ["Player"],
  "permissions": ["read:characters", "write:characters"]
}
```

#### PUT /api/users/me
Update current user's profile.

**Request**:
```json
{
  "firstName": "John",
  "lastName": "Smith",
  "displayName": "Johnny"
}
```

**Response**: `200 OK`

#### GET /api/users/{id}
Get user by ID.

**Response**: `200 OK`

#### GET /api/users
Get paginated list of users (admin only).

**Query Parameters**:
- `pageNumber` (default: 1)
- `pageSize` (default: 20)

**Response**: `200 OK`

## Integration Events

### Events Published

#### UserRegisteredEvent
Published when a new user registers.

```csharp
public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    DateTime RegisteredAtUtc
) : IntegrationEvent;
```

#### UserProfileUpdatedEvent
Published when a user updates their profile.

```csharp
public record UserProfileUpdatedEvent(
    Guid UserId,
    string FirstName,
    string LastName,
    string DisplayName,
    DateTime UpdatedAtUtc
) : IntegrationEvent;
```

#### UserRolesChangedEvent
Published when user roles are modified.

```csharp
public record UserRolesChangedEvent(
    Guid UserId,
    IReadOnlyCollection<string> NewRoles,
    DateTime ChangedAtUtc
) : IntegrationEvent;
```

## Authentication Flow

### 1. User Registration
```
User → POST /api/users/register
     → UsersModule creates user in Keycloak
     → UsersModule creates local user record
     → UserRegisteredEvent published
```

### 2. User Login
```
User → Keycloak /token endpoint
     → Keycloak validates credentials
     → Access token returned
     → User includes token in subsequent requests
```

### 3. Authenticated Request
```
User → GET /api/characters (with Bearer token)
     → API Gateway validates token
     → Permission check via IPermissionService
     → Request processed if authorized
```

## Authorization

### Role Hierarchy

```
Administrator
  ├── Full system access
  ├── User management
  └── All permissions

GameMaster
  ├── Campaign management
  ├── Character read/write
  └── Dice rolling

Player
  ├── Own character management
  ├── Dice rolling
  └── Campaign participation
```

### Permission Checking

```csharp
// In a command handler
public class DeleteCharacterHandler : ICommandHandler<DeleteCharacterCommand>
{
    private readonly IPermissionService _permissionService;
    
    public async Task<Result> Handle(
        DeleteCharacterCommand command, 
        CancellationToken cancellationToken)
    {
        var hasPermission = await _permissionService.HasPermissionAsync(
            command.UserId,
            "delete:characters",
            cancellationToken);
            
        if (!hasPermission)
            return Result.Failure(UserErrors.InsufficientPermissions);
            
        // Process deletion...
    }
}
```

## Security Considerations

1. **Password Policy**: Enforced by Keycloak configuration
2. **Token Expiration**: Configurable token lifetime
3. **Refresh Tokens**: Supported for long-lived sessions
4. **Rate Limiting**: Prevent brute force attacks
5. **Email Verification**: Optional email verification flow
6. **Two-Factor Authentication**: Supported via Keycloak

## Testing

### Running Tests

```bash
# Module tests (when available)
dotnet test src/api/modules/users/
```

### Example Tests

```csharp
[Fact]
public async Task RegisterUser_WithValidData_ShouldCreateUser()
{
    // Arrange
    var command = new RegisterUserCommand(
        "test@example.com",
        "Password123!",
        "Test",
        "User"
    );

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeEmpty();
}
```

## Configuration

### Keycloak Settings

```json
{
  "Keycloak": {
    "Authority": "http://localhost:8080/realms/tavern-trashers",
    "Realm": "tavern-trashers",
    "ClientId": "tavern-trashers-api",
    "ClientSecret": "your-client-secret",
    "RequireHttpsMetadata": false  // Development only
  }
}
```

### JWT Settings

```json
{
  "Jwt": {
    "Audience": "tavern-trashers-api",
    "Issuer": "http://localhost:8080/realms/tavern-trashers"
  }
}
```

## Usage Examples

### Register a New User

```csharp
var command = new RegisterUserCommand(
    "newplayer@example.com",
    "SecurePassword123!",
    "New",
    "Player"
);

var userId = await mediator.Send(command);
```

### Check User Permissions

```csharp
var hasPermission = await permissionService.HasPermissionAsync(
    userIdentityId,
    "write:characters"
);

if (!hasPermission)
{
    return Result.Failure(UserErrors.InsufficientPermissions);
}
```

### Get Current User

```csharp
var query = new GetCurrentUserQuery();
var user = await mediator.Send(query);
```

## Dependencies

- **Keycloak**: Identity provider
- **TavernTrashers.Api.Common.Domain**: Base entities
- **TavernTrashers.Api.Common.Application**: CQRS infrastructure
- **TavernTrashers.Api.Common.Infrastructure**: Database context
- **Entity Framework Core**: ORM
- **MediatR**: Command/Query handling

## Future Enhancements

- [ ] Social login (Google, Discord, etc.)
- [ ] Email verification workflow
- [ ] Password reset functionality
- [ ] User activity logging
- [ ] Session management
- [ ] Device tracking
- [ ] Account deletion/anonymization
- [ ] User preferences storage

## Related Documentation

- [API Reference](../../../docs/API_REFERENCE.md)
- [Security Policy](../../../SECURITY.md)
- [Development Guide](../../../docs/DEVELOPMENT.md)

## Contributing

When contributing to the Users module:

1. Follow security best practices
2. Never log sensitive information (passwords, tokens)
3. Validate all user input
4. Test authentication and authorization flows
5. Update integration events documentation

---

*Secure and scalable user management!* 🔐

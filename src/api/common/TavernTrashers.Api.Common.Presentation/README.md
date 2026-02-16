# Common Presentation Layer

The Common Presentation layer provides shared API infrastructure including endpoint patterns, authentication, result transformation, and API conventions.

## Overview

This library contains shared presentation concerns for building consistent REST APIs across all modules.

## Contents

```
TavernTrashers.Api.Common.Presentation/
├── Endpoints/          # Minimal API endpoint patterns
├── Authentication/     # Authentication extensions
├── Modules/            # Module endpoint registration
└── ApiResults.cs       # Result to HTTP response mapping
```

## Key Components

### API Results

**Result Mapping**: Converts domain results to HTTP responses
```csharp
public static class ApiResults
{
    public static IResult ToApiResult<T>(this Result<T> result)
    {
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : ToProblemDetails(result.Error);
    }
    
    public static IResult ToCreatedApiResult<T>(
        this Result<T> result, 
        string uri)
    {
        return result.IsSuccess
            ? Results.Created(uri, result.Value)
            : ToProblemDetails(result.Error);
    }
}
```

**Error Mapping**:
```csharp
// Validation errors → 400 Bad Request
// NotFound errors → 404 Not Found
// Conflict errors → 409 Conflict
// Other failures → 500 Internal Server Error
```

### Endpoint Patterns

**Standard Endpoint Structure**:
```csharp
public class GetRoll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dice/rolls/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetRollQuery(id);
            var result = await sender.Send(query, ct);
            return result.ToApiResult();
        })
        .WithName("GetRoll")
        .WithTags("Dice")
        .RequireAuthorization()
        .Produces<RollResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
```

### Authentication

**User Context**: Access current user
```csharp
public class UserContext : IUserContext
{
    public Guid UserId => /* from claims */;
    public string IdentityId => /* from claims */;
    public bool IsAuthenticated => /* check claims */;
}
```

## API Conventions

### Endpoint Naming

- **GET** `/api/{module}/{resource}` - List resources
- **GET** `/api/{module}/{resource}/{id}` - Get by ID
- **POST** `/api/{module}/{resource}` - Create resource
- **PUT** `/api/{module}/{resource}/{id}` - Update resource
- **DELETE** `/api/{module}/{resource}/{id}` - Delete resource

### Response Format

**Success**:
```json
{
  "data": { ... }
}
```

**Error**:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Character not found",
  "traceId": "00-abc..."
}
```

### Status Codes

- `200 OK` - Successful GET/PUT
- `201 Created` - Successful POST
- `204 No Content` - Successful DELETE
- `400 Bad Request` - Validation error
- `401 Unauthorized` - Not authenticated
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `409 Conflict` - Resource conflict
- `500 Internal Server Error` - Unexpected error

## Usage Examples

### Creating an Endpoint

```csharp
public class RollDice : IEndpoint
{
    public record Request(string Expression, object? Context);
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/dice/rolls", async (
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new RollDiceCommand(
                request.Expression,
                JsonSerializer.Serialize(request.Context));
                
            var result = await sender.Send(command, ct);
            
            return result.ToCreatedApiResult($"/api/dice/rolls/{result.Value}");
        })
        .WithName("RollDice")
        .WithTags("Dice")
        .RequireAuthorization()
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesValidationProblem();
    }
}
```

### Registering Module Endpoints

```csharp
public class DiceModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        new RollDice().MapEndpoint(app);
        new GetRoll().MapEndpoint(app);
        new GetRolls().MapEndpoint(app);
    }
}
```

## OpenAPI/Swagger

Endpoints automatically generate OpenAPI documentation:

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Tavern Trashers API", Version = "v1" });
});

app.UseSwagger();
app.UseSwaggerUI();
```

## Best Practices

### Endpoint Design

✅ **Do**:
- Use minimal APIs for simplicity
- Follow REST conventions
- Include OpenAPI metadata
- Require authorization by default
- Use proper HTTP status codes

❌ **Don't**:
- Put business logic in endpoints
- Return entities directly (use DTOs)
- Ignore error handling
- Skip authentication requirements

### DTO Mapping

```csharp
// In endpoint
var result = await sender.Send(query, ct);
var response = new CharacterResponse(
    result.Value.Id,
    result.Value.Name,
    result.Value.Level);
return Results.Ok(response);
```

## Testing Endpoints

```csharp
[Fact]
public async Task GetRoll_WithValidId_ShouldReturn200()
{
    // Arrange
    var client = _factory.CreateClient();
    var rollId = await CreateTestRoll();
    
    // Act
    var response = await client.GetAsync($"/api/dice/rolls/{rollId}");
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var roll = await response.Content.ReadFromJsonAsync<RollResponse>();
    roll.Should().NotBeNull();
}
```

## Dependencies

- **ASP.NET Core**: Minimal APIs
- **MediatR**: Command/Query dispatch
- **TavernTrashers.Api.Common.Application**: Application layer

## Related Documentation

- [API Reference](../../../../docs/API_REFERENCE.md)
- [Common Application Layer](../TavernTrashers.Api.Common.Application/README.md)

---

*Clean APIs, happy developers!* 🌐

# API Reference

Complete API documentation for Tavern Trashers REST endpoints.

## Table of Contents

- [Authentication](#authentication)
- [Base URL](#base-url)
- [Common Headers](#common-headers)
- [Response Format](#response-format)
- [Error Handling](#error-handling)
- [Modules](#modules)
  - [Dice Module](#dice-module)
  - [Users Module](#users-module)
  - [Characters Module](#characters-module)

## Authentication

Tavern Trashers uses OAuth 2.0 / OpenID Connect via Keycloak for authentication.

### Getting an Access Token

```http
POST http://localhost:8080/realms/tavern-trashers/protocol/openid-connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password
&client_id=tavern-trashers-client
&username=<username>
&password=<password>
```

### Using the Token

Include the token in the Authorization header:

```http
Authorization: Bearer <access_token>
```

## Base URL

**Development**: `http://localhost:5000/api`

**Production**: `https://api.taverntrashers.com/api` (when deployed)

## Common Headers

All requests should include:

```http
Content-Type: application/json
Accept: application/json
Authorization: Bearer <access_token>
```

## Response Format

### Success Response

```json
{
  "data": { ... },
  "isSuccess": true,
  "error": null
}
```

### Error Response

```json
{
  "data": null,
  "isSuccess": false,
  "error": {
    "code": "ERROR_CODE",
    "message": "Human-readable error message",
    "type": "Validation | NotFound | Conflict | Failure"
  }
}
```

## Error Handling

### HTTP Status Codes

- `200 OK`: Successful request
- `201 Created`: Resource created successfully
- `204 No Content`: Successful request with no content
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `409 Conflict`: Resource conflict
- `500 Internal Server Error`: Server error

### Error Types

- **Validation**: Input validation failed
- **NotFound**: Requested resource doesn't exist
- **Conflict**: Resource state conflict
- **Failure**: Operation failed

## Modules

---

## Dice Module

Handles dice rolling, roll history, and dice notation parsing.

### Roll Dice

Roll dice using standard dice notation.

**Endpoint**: `POST /api/dice/rolls`

**Request Body**:
```json
{
  "expression": "2d20+5",
  "context": {
    "character": "Aragorn",
    "action": "Attack roll"
  }
}
```

**Supported Dice Notation**:
- Basic rolls: `d20`, `2d6`, `3d8+5`
- Keep highest/lowest: `4d6kh3`, `4d6kl1`
- Drop highest/lowest: `4d6dh1`, `4d6dl1`
- Reroll: `2d20r1` (reroll 1s)
- Exploding dice: `2d6!` (reroll on max)
- Advantage/Disadvantage: `2d20kh1` (advantage), `2d20kl1` (disadvantage)
- Complex expressions: `(2d6+3)*2`, `d20+5+d4`

**Response**: `201 Created`
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "expression": "2d20+5",
    "total": 27,
    "minimum": 7,
    "maximum": 45,
    "average": 26.0,
    "rolledAtUtc": "2024-01-15T10:30:00Z",
    "rawRolls": [
      { "die": 20, "value": 15 },
      { "die": 20, "value": 7 }
    ],
    "keptRolls": [
      { "die": 20, "value": 15 },
      { "die": 20, "value": 7 }
    ],
    "context": {
      "character": "Aragorn",
      "action": "Attack roll"
    }
  },
  "isSuccess": true
}
```

### Get Roll by ID

Retrieve a specific roll by its ID.

**Endpoint**: `GET /api/dice/rolls/{id}`

**Parameters**:
- `id` (UUID): Roll identifier

**Response**: `200 OK`
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "expression": "2d20+5",
    "total": 27,
    "minimum": 7,
    "maximum": 45,
    "average": 26.0,
    "rolledAtUtc": "2024-01-15T10:30:00Z",
    "rawRolls": [...],
    "keptRolls": [...],
    "context": {...}
  },
  "isSuccess": true
}
```

### Get Roll History

Get paginated roll history.

**Endpoint**: `GET /api/dice/rolls`

**Query Parameters**:
- `pageNumber` (int, default: 1): Page number
- `pageSize` (int, default: 20): Items per page
- `sortBy` (string): Sort field (e.g., "rolledAtUtc")
- `sortDescending` (bool, default: true): Sort direction

**Response**: `200 OK`
```json
{
  "data": {
    "items": [
      {
        "id": "...",
        "expression": "2d20+5",
        "total": 27,
        "rolledAtUtc": "2024-01-15T10:30:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 150,
    "totalPages": 8,
    "hasPreviousPage": false,
    "hasNextPage": true
  },
  "isSuccess": true
}
```

### Reroll Dice

Reroll an existing roll with the same expression.

**Endpoint**: `POST /api/dice/rolls/{id}/reroll`

**Parameters**:
- `id` (UUID): Original roll ID

**Response**: `201 Created`
```json
{
  "data": {
    "id": "new-roll-id",
    "expression": "2d20+5",
    "total": 31,
    "parent": {
      "id": "original-roll-id",
      "total": 27
    },
    ...
  },
  "isSuccess": true
}
```

---

## Users Module

Manages user accounts, profiles, and authentication.

### Get Current User

Get the authenticated user's profile.

**Endpoint**: `GET /api/users/me`

**Response**: `200 OK`
```json
{
  "data": {
    "id": "user-id",
    "username": "player1",
    "email": "player1@example.com",
    "displayName": "Player One",
    "createdAtUtc": "2024-01-01T00:00:00Z",
    "roles": ["Player", "GameMaster"]
  },
  "isSuccess": true
}
```

### Update User Profile

Update user profile information.

**Endpoint**: `PUT /api/users/me`

**Request Body**:
```json
{
  "displayName": "New Display Name",
  "email": "newemail@example.com"
}
```

**Response**: `200 OK`

### Get User by ID

Get a user's public profile.

**Endpoint**: `GET /api/users/{id}`

**Response**: `200 OK`
```json
{
  "data": {
    "id": "user-id",
    "username": "player1",
    "displayName": "Player One"
  },
  "isSuccess": true
}
```

---

## Characters Module

Manages character creation, stats, and inventory.

### Create Character

Create a new character.

**Endpoint**: `POST /api/characters`

**Request Body**:
```json
{
  "name": "Aragorn",
  "classId": "class-id",
  "level": 1,
  "attributes": {
    "strength": 16,
    "dexterity": 14,
    "constitution": 15,
    "intelligence": 12,
    "wisdom": 13,
    "charisma": 14
  }
}
```

**Response**: `201 Created`
```json
{
  "data": {
    "id": "character-id",
    "name": "Aragorn",
    "classId": "class-id",
    "level": 1,
    "attributes": {...},
    "createdAtUtc": "2024-01-15T10:00:00Z"
  },
  "isSuccess": true
}
```

### Get Character

Get character details.

**Endpoint**: `GET /api/characters/{id}`

**Response**: `200 OK`

### Update Character

Update character information.

**Endpoint**: `PUT /api/characters/{id}`

**Request Body**:
```json
{
  "name": "Aragorn, King of Gondor",
  "level": 20
}
```

**Response**: `200 OK`

### Delete Character

Delete a character.

**Endpoint**: `DELETE /api/characters/{id}`

**Response**: `204 No Content`

### List Characters

Get user's characters.

**Endpoint**: `GET /api/characters`

**Query Parameters**:
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 20)

**Response**: `200 OK`

---

## Rate Limiting

API endpoints are rate-limited to prevent abuse:

- **Authenticated requests**: 1000 requests per hour
- **Unauthenticated requests**: 100 requests per hour

Rate limit headers:
```http
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 950
X-RateLimit-Reset: 1642252800
```

## Pagination

Paginated endpoints return:

```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 100,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

## Filtering and Sorting

Many endpoints support filtering and sorting:

**Example**:
```http
GET /api/characters?sortBy=level&sortDescending=true&filterBy=class&filterValue=warrior
```

## Webhooks

Webhook support (planned for future release):

- Roll events
- Character updates
- User events

## SDK and Client Libraries

Official client libraries (planned):

- .NET SDK
- TypeScript/JavaScript SDK
- Python SDK

## Versioning

API versioning strategy:

- **URL versioning**: `/api/v1/`, `/api/v2/`
- **Current version**: v1 (implicit)
- **Deprecation policy**: 6 months notice

## OpenAPI/Swagger

Interactive API documentation available at:

**Development**: http://localhost:5000/swagger

Download OpenAPI specification:
```http
GET /swagger/v1/swagger.json
```

## Testing the API

### Using cURL

```bash
# Roll dice
curl -X POST http://localhost:5000/api/dice/rolls \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{"expression": "2d20+5"}'

# Get roll history
curl http://localhost:5000/api/dice/rolls \
  -H "Authorization: Bearer <token>"
```

### Using Postman

Import the OpenAPI specification into Postman for a complete collection.

## Best Practices

1. **Always authenticate**: Include Authorization header
2. **Handle errors**: Check `isSuccess` and handle `error` object
3. **Respect rate limits**: Implement backoff strategies
4. **Use pagination**: Don't fetch all results at once
5. **Validate input**: Client-side validation before API calls
6. **Cache responses**: Cache GET requests when appropriate

## Support

For API issues or questions:
- **GitHub Issues**: Report bugs
- **GitHub Discussions**: Ask questions
- **Documentation**: Check [docs/](../docs/)

---

*This API reference is maintained alongside the codebase. Last updated: 2024*

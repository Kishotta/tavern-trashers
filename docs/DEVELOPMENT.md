# Development Guide

This guide provides comprehensive instructions for setting up, developing, and contributing to Tavern Trashers.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Development Environment Setup](#development-environment-setup)
- [Project Structure](#project-structure)
- [Development Workflow](#development-workflow)
- [Building and Running](#building-and-running)
- [Testing](#testing)
- [Debugging](#debugging)
- [Database Management](#database-management)
- [Code Quality](#code-quality)

## Prerequisites

### Required Software

- **.NET 8.0 SDK or later**: [Download](https://dotnet.microsoft.com/download)
- **Node.js 18+ and npm**: [Download](https://nodejs.org/)
- **Docker Desktop**: [Download](https://www.docker.com/products/docker-desktop) (for Aspire orchestration)
- **Git**: [Download](https://git-scm.com/)

### Recommended Tools

- **Visual Studio 2022+** or **JetBrains Rider** (for .NET development)
- **Visual Studio Code** (for TypeScript/Angular development)
- **Azure Data Studio** or **pgAdmin** (for database management)
- **Postman** or **Insomnia** (for API testing)

### Optional Tools

- **Docker** and **Docker Compose** (if not using Aspire)
- **Redis Insight** (for cache inspection)
- **RabbitMQ Management** (for message queue monitoring)

## Development Environment Setup

### 1. Clone the Repository

```bash
git clone https://github.com/Kishotta/tavern-trashers.git
cd tavern-trashers
```

### 2. Backend Setup (.NET)

#### Restore Dependencies

```bash
cd src
dotnet restore
```

#### Build Solution

```bash
dotnet build src/TavernTrashers.slnx
```

#### Verify Build

```bash
dotnet build --configuration Release
```

### 3. Frontend Setup (Angular)

#### Install Dependencies

```bash
cd src/web/tavern-trashers-web
npm install
```

#### Verify Installation

```bash
npm run build
```

### 4. Infrastructure Setup (Aspire)

The easiest way to run the entire stack:

```bash
cd src/aspire
dotnet run --project TavernTrashers.AppHost
```

This starts:
- PostgreSQL (port 5432)
- Redis (port 6379)
- RabbitMQ (port 5672, management UI on 15672)
- Keycloak (port 8080)
- API Gateway
- Backend API
- Frontend application

**Aspire Dashboard**: Available at `http://localhost:15000` for monitoring.

## Project Structure

```
tavern-trashers/
├── .github/                 # GitHub workflows, issue templates
├── docs/                    # Documentation
├── src/
│   ├── api/                 # Backend API
│   │   ├── common/          # Shared libraries
│   │   │   ├── Domain/      # Common domain logic
│   │   │   ├── Application/ # Common application logic
│   │   │   ├── Infrastructure/ # Shared infrastructure
│   │   │   └── Presentation/   # Shared API presentation
│   │   ├── modules/         # Feature modules
│   │   │   ├── dice/        # Dice rolling module
│   │   │   ├── users/       # User management module
│   │   │   └── characters/  # Character management module
│   │   ├── TavernTrashers.Api/        # Main API entry point
│   │   ├── TavernTrashers.Gateway/    # API Gateway (YARP)
│   │   └── TavernTrashers.MigrationService/ # Database migrations
│   ├── aspire/              # .NET Aspire orchestration
│   │   ├── TavernTrashers.AppHost/        # App host
│   │   └── TavernTrashers.ServiceDefaults/ # Service defaults
│   └── web/                 # Frontend applications
│       └── tavern-trashers-web/  # Angular web app
├── README.md
├── CONTRIBUTING.md
├── LICENSE.md
└── SECURITY.md
```

## Development Workflow

### 1. Create a Feature Branch

```bash
git checkout -b feature/your-feature-name
```

### 2. Make Changes

Follow the [coding conventions](#coding-conventions) and write tests.

### 3. Test Your Changes

```bash
# Run all tests
dotnet test src/TavernTrashers.slnx

# Run specific test project
dotnet test src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Domain.Tests
```

### 4. Commit Changes

```bash
git add .
git commit -m "feat: add new feature"
```

Follow [Conventional Commits](https://www.conventionalcommits.org/):
- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `refactor:` Code refactoring
- `test:` Adding or updating tests
- `chore:` Maintenance tasks

### 5. Push and Create Pull Request

```bash
git push origin feature/your-feature-name
```

Then create a pull request on GitHub.

## Building and Running

### Full Stack (Recommended)

```bash
dotnet run --project src/aspire/TavernTrashers.AppHost
```

Access:
- **Aspire Dashboard**: http://localhost:15000
- **API**: http://localhost:5000
- **Frontend**: http://localhost:4200
- **Keycloak**: http://localhost:8080

### Backend Only

```bash
# Run API directly
dotnet run --project src/api/TavernTrashers.Api

# Or with hot reload
dotnet watch --project src/api/TavernTrashers.Api
```

### Frontend Only

```bash
cd src/web/tavern-trashers-web
npm start
```

Access at http://localhost:4200

### Individual Modules

Each module can be developed independently:

```bash
dotnet build src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Domain
```

## Testing

### Unit Tests

```bash
# All tests
dotnet test

# Specific project
dotnet test src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Domain.Tests

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Integration Tests

Integration tests require infrastructure (database, cache, etc.):

```bash
# Start infrastructure first
dotnet run --project src/aspire/TavernTrashers.AppHost

# Run integration tests
dotnet test --filter "Category=Integration"
```

### Frontend Tests

```bash
cd src/web/tavern-trashers-web

# Unit tests
npm test

# E2E tests
npm run e2e
```

## Debugging

### Backend Debugging

#### Visual Studio
1. Set `TavernTrashers.AppHost` as startup project
2. Press F5 to start debugging
3. Set breakpoints in any module

#### VS Code
1. Open the workspace
2. Use the provided launch configurations
3. Select "Debug Aspire App" configuration

#### Command Line
```bash
dotnet run --project src/aspire/TavernTrashers.AppHost --launch-profile https
```

### Frontend Debugging

#### VS Code
1. Install "Debugger for Chrome" extension
2. Run `npm start` in terminal
3. Use "Debug Angular" launch configuration

#### Browser DevTools
- Chrome/Edge DevTools (F12)
- Source maps enabled by default in development

## Database Management

### Migrations

Migrations run automatically with the `TavernTrashers.MigrationService` when using Aspire.

#### Manual Migration Commands

```bash
# Add migration (example for Dice module)
dotnet ef migrations add MigrationName --project src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Infrastructure

# Update database
dotnet ef database update --project src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Infrastructure

# List migrations
dotnet ef migrations list --project src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Infrastructure
```

### Database Access

#### Connection String (Development)
```
Server=localhost;Port=5432;Database=taverntrashers;User Id=postgres;Password=postgres;
```

#### Using pgAdmin
1. Open pgAdmin (http://localhost:5050 if using Aspire)
2. Connect to localhost:5432
3. Database: taverntrashers

#### Using Azure Data Studio
1. Connect to PostgreSQL
2. Server: localhost
3. Database: taverntrashers

### Seeding Data

```bash
# Seed data is handled by the migration service
# Custom seed data can be added to DbContext.OnModelCreating
```

## Code Quality

### Linting

#### .NET
```bash
# Restore tools
dotnet tool restore

# Run analyzers
dotnet build /p:TreatWarningsAsErrors=true
```

#### Angular
```bash
cd src/web/tavern-trashers-web

# Lint
npm run lint

# Format with Prettier
npm run format
```

### Code Style

- **C#**: Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- **TypeScript**: Follow [Angular Style Guide](https://angular.dev/style-guide)
- **Consistent naming**: PascalCase for C#, camelCase for TypeScript

### Pre-commit Checks

Before committing:
1. Build solution: `dotnet build`
2. Run tests: `dotnet test`
3. Check formatting: `npm run format`
4. Review changes: `git diff`

## Coding Conventions

### Backend

#### Modular Structure
Each module follows clean architecture:
- **Domain**: Core business logic, entities, value objects
- **Application**: Use cases, commands, queries, event handlers
- **Infrastructure**: Data access, external services
- **Presentation**: API controllers, DTOs

#### Naming Conventions
- **Entities**: Singular nouns (e.g., `Character`, `Roll`)
- **Commands**: Verb + Noun (e.g., `CreateCharacter`, `RollDice`)
- **Queries**: Get + Noun (e.g., `GetCharacter`, `GetRolls`)
- **Events**: Past tense (e.g., `CharacterCreated`, `DiceRolled`)

#### Error Handling
- Use `Result<T>` pattern for expected errors
- Throw exceptions only for unexpected errors
- Create specific error types per module

### Frontend

#### Component Structure
```typescript
@Component({
  selector: 'app-feature-name',
  standalone: true,
  templateUrl: './feature-name.component.html',
  styleUrls: ['./feature-name.component.scss']
})
```

#### State Management
- Use NgRx for complex state
- Use signals for simple component state
- Follow unidirectional data flow

## Environment Variables

### Backend

Key environment variables in `appsettings.json` or environment:

```json
{
  "ConnectionStrings": {
    "Database": "Server=localhost;Port=5432;Database=taverntrashers;..."
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672
  },
  "Keycloak": {
    "Authority": "http://localhost:8080/realms/tavern-trashers"
  }
}
```

### Frontend

Environment files in `src/web/tavern-trashers-web/src/environments/`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  keycloakUrl: 'http://localhost:8080'
};
```

## Troubleshooting

### Common Issues

#### Port Already in Use
```bash
# Find process using port
lsof -i :5000  # macOS/Linux
netstat -ano | findstr :5000  # Windows

# Kill process
kill -9 <PID>  # macOS/Linux
taskkill /PID <PID> /F  # Windows
```

#### Database Connection Failed
1. Ensure PostgreSQL is running
2. Verify connection string
3. Check firewall settings

#### Migration Errors
```bash
# Drop and recreate database (development only!)
dotnet ef database drop --force
dotnet run --project src/aspire/TavernTrashers.AppHost
```

#### Angular Build Errors
```bash
# Clear cache
rm -rf node_modules package-lock.json
npm install
npm run build
```

## Performance Tips

### Development Performance

1. **Use Aspire**: Faster than individual Docker containers
2. **Hot Reload**: Use `dotnet watch` for backend
3. **Incremental Builds**: Build specific projects instead of entire solution
4. **Parallel Testing**: `dotnet test --parallel`

### Database Performance

1. **Use DbContextPool**: Already configured
2. **Eager Loading**: Use `Include()` to avoid N+1 queries
3. **Indexing**: Add indexes for frequently queried columns
4. **Connection Pooling**: Enabled by default

## Additional Resources

- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Angular Documentation](https://angular.dev)
- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

## Getting Help

- **GitHub Discussions**: Ask questions and share ideas
- **Issue Tracker**: Report bugs and request features
- **Documentation**: Check [docs/](../docs/) folder

---

Happy coding! 🎲

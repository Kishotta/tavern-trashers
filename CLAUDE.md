# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Backend (.NET)
- **Build entire solution:** `dotnet build src/TavernTrashers.sln`
- **Run API directly:** `dotnet run --project src/api/TavernTrashers.Api`
- **Run with Aspire (recommended):** `dotnet run --project src/aspire/TavernTrashers.AppHost`
- **Run tests:** `dotnet test src/TavernTrashers.sln`
- **Run specific test project:** `dotnet test src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Domain.Tests`
- **Database migrations:** Use `TavernTrashers.MigrationService` project (runs automatically with Aspire)

### Frontend (Angular)
- **tavern-trashers app:** Navigate to `src/web/tavern-trashers/` then `npm start`
- **tavern-trashers-web app:** Navigate to `src/web/tavern-trashers-web/` then `npm start`
- **Build:** `npm run build`
- **Tests:** `npm test`
- **Format:** `npm run format` (uses Prettier)

### Full Stack Development
- **Recommended:** Run `dotnet run --project src/aspire/TavernTrashers.AppHost` to start entire stack including PostgreSQL, Redis, RabbitMQ, Keycloak, and web frontend

## Architecture Overview

### Modular Monolith Structure
The backend follows a modular monolith pattern with clean architecture principles:

- **Domain Layer:** Core business logic and entities
- **Application Layer:** Use cases, command/query handlers, integration events
- **Infrastructure Layer:** Data access, external services, messaging
- **Presentation Layer:** Controllers, API endpoints, DTOs

### Module Organization
Located in `src/api/modules/`:
- **Campaigns:** Campaign management and game sessions
- **Dice:** Dice rolling mechanics and history  
- **Encounters:** Encounter management and combat scenarios
- **Users:** User management and authentication

Each module follows the same layered structure: Domain → Application → Infrastructure → Presentation

**Note:** The Characters module has been removed from the current codebase. The Encounters module is newly added and includes PublicApi projects for inter-module communication.

### Common Components
Located in `src/api/common/`:
- **Domain:** Base entities, value objects, domain events
- **Application:** Base behaviors, pipelines, caching
- **Infrastructure:** Database contexts, message brokers
- **Presentation:** Base controllers, API conventions
- **SourceGenerators:** Code generation utilities

### Key Infrastructure
- **Database:** PostgreSQL with Entity Framework Core using DbContextPool
- **Caching:** Redis for performance optimization  
- **Messaging:** RabbitMQ for inter-module communication
- **Identity:** Keycloak for authentication/authorization
- **Gateway:** YARP-based API gateway for routing

### Frontend Applications
- **tavern-trashers:** Angular 19 app using PrimeNG components and Keycloak authentication
- **tavern-trashers-web:** Angular 19 app using NgRx state management and OAuth2/OIDC

Both apps use cross-platform npm scripts with `run-script-os` for Windows/Unix compatibility.

## Development Guidelines

### Adding New Modules
1. Create module structure in `src/api/modules/[module-name]/`
2. Follow standard layers: Domain → Application → Infrastructure → Presentation  
3. Add module to `ModuleRepository.cs` in the main API project
4. Use `[GenerateModuleBoilerplate]` attribute for consistent module setup
5. Consider PublicApi projects for cross-module communication
6. Consider integration events for cross-module communication

### Database Changes
- Entity changes trigger automatic migrations via `TavernTrashers.MigrationService`
- Uses repository pattern with DbContextPool for performance
- Each module can have its own DbContext if needed

### Testing
- Domain tests focus on business logic validation
- Follow naming convention: `[ModuleName].Domain.Tests`
- Run tests before committing changes

### Code Organization
- Use source generators for repetitive code patterns
- Follow existing naming conventions and folder structure
- Integration events should be in separate projects for clear module boundaries

### Infrastructure Setup
The Aspire AppHost (`src/aspire/TavernTrashers.AppHost`) orchestrates the entire stack:
- PostgreSQL database with PgAdmin
- Redis cache with RedisInsight
- RabbitMQ message queue with management UI
- Keycloak identity server
- API gateway and backend services
- Angular web frontend

All infrastructure components use persistent volumes and are configured to wait for dependencies before starting.
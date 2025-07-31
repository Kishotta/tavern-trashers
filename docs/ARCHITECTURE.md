# Architecture Overview

Tavern Trashers is a modular monolith built with .NET for the backend and Angular for the frontend. The system is designed for extensibility, maintainability, and clear separation of concerns.

## High-Level Structure
- **Backend:** .NET modular monolith, organized by feature modules (Campaigns, Characters, Dice, Users, etc.)
- **Frontend:** Angular standalone applications, communicating with the backend via REST APIs

## Backend (src/)
- `api/` contains the main API and modules
  - `common/` shared logic, domain, infrastructure, presentation, source generators, and testing
  - `modules/` feature modules (campaigns, characters, dice, users)
  - `TavernTrashers.Api/` main API project, configuration, and entry point
  - `TavernTrashers.Gateway/` API gateway for routing and aggregation
  - `TavernTrashers.MigrationService/` handles database migrations
- `aspire/` contains app host and service defaults

## Frontend (web/)
- `tavern-trashers/` Angular app for main user experience
- `tavern-trashers-web/` (future/alternate web client)

## Data & Logic Flow
1. **Frontend** sends HTTP requests to the API Gateway or directly to the API.
2. **API Gateway** routes requests to the appropriate module or aggregates responses.
3. **API Modules** handle business logic, validation, and data access.
4. **Database** operations are managed by the infrastructure layer.

## Extending the System
- Add new modules under `src/api/modules/`
- Register modules in the main API project
- Frontend can be extended with new Angular features/components

## See Also
- [User Guide](./USER_GUIDE.md)
- [Module Overview](./MODULES.md)

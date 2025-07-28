# Backend API

The backend of Tavern Trashers follows a modular, Domain‑Driven Design (DDD) approach.  Each domain (for example, dice or users) lives under `modules/` and is split into four projects:

* `*.Domain` – entities, value objects, domain services and events.  Free of any frameworks.
* `*.Application` – application services, commands, queries and event handlers.  Depends on domain.
* `*.Infrastructure` – data access, external integrations, message brokers.  Depends on application and domain.
* `*.Presentation` – minimal API endpoints and controllers.  Depends on application.

The root API host (`TavernTrashers.Api`) composes modules by scanning assemblies and registers shared services like exception handling and OpenAPI.  The gateway (`TavernTrashers.Gateway`) acts as a reverse proxy to route front‑end requests to the appropriate module.

## Running only the API

If you want to run just the backend without Aspire, you can run:

```
dotnet run --project src/api/TavernTrashers.Api/TavernTrashers.Api.csproj
```

This will start the API at the configured port (see `appsettings.json`).  Use `dotnet run` on `TavernTrashers.Gateway` to start the gateway.

## Adding a new module

1. Create a new folder under `src/api/modules/<module‑name>/` with sub‑projects `<Module>.Domain`, `<Module>.Application`, `<Module>.Infrastructure` and `<Module>.Presentation`.
2. Reference `TavernTrashers.Api.Common.Domain`, `.Application`, `.Infrastructure` and `.Presentation` as needed.
3. Define your entities and events in the domain project, then implement your use cases in the application project.
4. Register your module in the API host by adding an extension method in `TavernTrashers.Api.Extensions` and calling it from `Program.cs`.

# Tavern Trashers

Tavern Trashers is a Dungeons & Dragons companion application that aims to reduce friction at the table by providing digital tools for both players and dungeon masters.  The project is currently a work‑in‑progress: the backend uses ASP.NET Core 9.0 with a micro‑service architecture orchestrated via the .NET Aspire toolchain, and the frontend is written in Angular.

## Project goals

* Provide a web‑based home for character sheets, spells, items and other player resources.
* Offer a robust dice engine that supports both standard polyhedral dice and FATE‑style dice rolls.
* Manage user accounts and authentication via Keycloak.
* Persist campaign data in a PostgreSQL database, accelerate queries with Redis and handle asynchronous integration events through RabbitMQ.
* Expose an API gateway that unifies the micro‑services, and a browser‑based client.

## Architecture overview

The solution follows a modular, Domain‑Driven design.  The `src/api` folder contains the backend micro‑services, while `src/web/tavern-trashers-web` houses the Angular client and `src/aspire` contains the Aspire application that ties everything together.

| Layer | Description |
|------|-------------|
| **Service host** (`src/api/TavernTrashers.Api`) | An ASP.NET Core minimal API that configures modules, middleware and OpenAPI. |
| **Gateway** (`src/api/TavernTrashers.Gateway`) | A lightweight proxy that routes client requests to the appropriate service. |
| **Modules** (`src/api/modules`) | Domain‑specific services. Current modules include a *Dice* module for rolling dice and a *Users* module for user management. Each module has Domain, Application, Infrastructure and Presentation projects. |
| **Common libraries** (`src/api/common`) | Shared abstractions such as `Entity<T>`, result types, and event bus interfaces that underpin the modules. |
| **Infrastructure** (`src/aspire`) | The .NET Aspire application describes the deployment topology, including Postgres, Redis, RabbitMQ, Keycloak, the API host, gateway and web client, and orchestrates their startup. |
| **Frontend** (`src/web/tavern-trashers-web`) | The Angular client that consumes the gateway API and provides the user interface. |

### Dependencies

The backend targets **.NET 9** with nullable reference types and treats compiler warnings as errors.  The solution references the `SonarAnalyzer.CSharp` package to enforce code quality.  The distributed application uses Aspire to spin up Postgres, Redis, RabbitMQ and Keycloak containers and to host the API, gateway and Angular web application.

### Development workflow

To run the entire system locally:

```bash
# from the repository root
cd src/aspire/TavernTrashers.AppHost
dotnet run
```

The Aspire app will provision local containers for Postgres, Redis, RabbitMQ and Keycloak, run any EF Core migrations and then launch the API, gateway and Angular web app.  Once started, navigate to `http://localhost:4200` for the web client or the configured gateway port for the API.  The environment variables and ports are defined in the Aspire `Program.cs`.

Alternatively, to run only the frontend:

```bash
cd src/web/tavern-trashers-web
npm install
ng serve
```

### Directory layout

* `src/api` – backend projects:
  * `TavernTrashers.Api` – minimal API host.
  * `TavernTrashers.Gateway` – API gateway.
  * `common/` – shared domain/application/infrastructure/presentation code.
  * `modules/` – domain modules such as dice and users.
* `src/aspire` – distributed application definition using Aspire.
* `src/web/tavern-trashers-web` – Angular client.

### Contributing

We welcome contributions!  Please see [`CONTRIBUTING.md`](CONTRIBUTING.md) for guidelines on setting up your environment, coding standards, and the preferred workflow for creating issues and pull requests.

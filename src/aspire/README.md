# Aspire Distributed Application

The `aspire` folder contains the [Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/) app host that orchestrates the distributed Tavern Trashers environment. It wires up infrastructure containers and starts each service in the correct order.

## Composition

The `TavernTrashers.AppHost` project defines the distributed system:

- **Postgres** – relational database for API data.
- **Redis** – distributed cache for sessions and general caching.
- **RabbitMQ** – message broker for asynchronous events between modules.
- **Keycloak** – identity and access management provider for OAuth2/OpenID Connect.
- **TavernTrashers.MigrationService** – runs database migrations before the API starts.
- **TavernTrashers.Api** – the ASP.NET Core backend implementing the modules.
- **TavernTrashers.Gateway** – YARP reverse proxy.
- **tavern-trashers-web** – Angular single-page application served behind the gateway.

## Running the system

To start the full system locally:

```bash
cd src/aspire/TavernTrashers.AppHost
dotnet run
```

Aspire uses containers to provision Postgres, Redis, RabbitMQ and Keycloak automatically. Once all services are ready, the Angular app will be accessible through the gateway at `https://localhost:8080` (port may vary according to your configuration).

## Adding Services

When new modules or services are added to the solution, register them in `TavernTrashers.AppHost` by adding a resource and appropriate wiring (e.g., environment variables, connection strings). See the existing code for examples.

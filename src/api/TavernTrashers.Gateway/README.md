# API Gateway

The API Gateway project uses YARP (Yet Another Reverse Proxy) to route requests to the backend services. It acts as a single entry point for the Angular web client and any other HTTP clients.

## Purpose

- Provide a unified HTTP endpoint for the Tavern Trashers API.
- Handle cross-cutting concerns like authentication, authorization, logging and rate limiting (future enhancements).
- Simplify CORS configuration by centralising all API routes under one host.

## Configuration

The gateway's routes and clusters are defined in `TavernTrashersGateway.yarp.json`. Each module’s endpoints are registered under a specific route prefix (e.g., `/dice`, `/users`, etc.). When new API modules are added to the project, corresponding routes should be added to the YARP configuration.

## Running

The gateway runs as part of the Aspire distributed app. When running locally, ASP.NET is configured to serve the Angular SPA from `src/web/tavern-trashers-web/dist` in production mode, or to proxy to `ng serve` in development.

# Tavern Trashers Web Client

This directory contains the Angular front‑end for the Tavern Trashers project. The web client provides a responsive user interface for interacting with the Tavern Trashers API, including rolling dice, managing users and exploring modules as they are developed.

## Technology stack

- **Angular** (v16+) – TypeScript framework for building single‑page applications.
- **Angular CLI** – Provides commands for scaffolding, building, testing and serving the app.
- **RxJS** – Reactive programming library used for handling asynchronous streams.
- **Bootstrap/Tailwind** – CSS framework for styling the UI (adjust to match the project’s chosen library).
- Communicates with the backend solely through the API gateway at `/api`.

## Project structure

The Angular app lives under the `src` folder. Key directories include:

- `src/app/`: root Angular module.
- `src/app/features/`: feature modules encapsulating functional domains (e.g. `dice`, `users`).
- `src/app/core/`: shared services such as API clients and authentication guards.
- `src/app/shared/`: reusable UI components.
- `src/environments/`: environment configuration for development vs production builds.

## Development

Ensure you have Node.js and the Angular CLI installed. To install dependencies and run the dev server:

```bash
npm install
ng serve
```

This will launch the app at `http://localhost:4200/`. The development proxy to the API gateway is configured in `proxy.conf.js`, ensuring requests starting with `/api` are forwarded to the local gateway (running on port 8001 by default). Adjust the `proxy.conf.js` file if your gateway runs on a different port.

### Unit tests

Angular uses **Karma** and **Jasmine** for unit tests. Run:

```bash
ng test
```

Test files live alongside their components in `*.spec.ts` files.

### End‑to‑end tests

To run end‑to‑end (E2E) tests using **Cypress** or your chosen framework:

```bash
ng e2e
```

E2E tests are defined in the `e2e/` folder and simulate user interactions against a running instance of the app.

### Build

To build the application for production:

```bash
ng build
```

The output will be placed in the `dist/` directory. Production builds are optimized and minified. The resulting assets can be served by any static web server.

## Adding features

Feature modules should live under `src/app/features/` and expose their own routing and services. Use Angular CLI generators to scaffold new modules and components to maintain consistency:

```bash
ng generate module features/my-feature --route my-feature --module app
ng generate component features/my-feature
```

Ensure API calls go through the centralized API client in `src/app/core` and avoid hard‑coding endpoints.

## Deployment

The Tavern Trashers web client is intended to run behind the YARP gateway. In production, set environment variables or update `environment.ts` and `environment.prod.ts` to point the client at the correct gateway URL.

---

Feel free to contribute to the web client by submitting issues or pull requests!

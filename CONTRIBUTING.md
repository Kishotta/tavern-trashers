# Contributing to Tavern Trashers

Thanks for your interest in contributing to Tavern Trashers!  This project is early in development, and we welcome help to shape its direction.  The project has two major portions: a .NET microservice backend and an Angular frontend.  To contribute, please follow these steps.

## Development environment

* **.NET 9 SDK** – used for the backend.  Install from the [.NET download page](https://dotnet.microsoft.com/download).
* **Node.js (v18+) and npm** – required to build the Angular frontend.
* **Docker** – optional, used by Aspire to run Postgres, Redis, RabbitMQ and Keycloak locally.

After cloning the repository, restore dependencies:

```bash
dotnet restore
npm install --prefix src/web/tavern-trashers-web
```

## Branching model

1. Fork the repository (or create a feature branch if you have push access).
2. Create a descriptive branch name from `main`.  Example: `feature/add-user-registration`.
3. Make your changes, following the code style guidelines below.
4. Add or update tests where appropriate.
5. Run `dotnet build` and `dotnet test` and ensure the Angular app builds with `ng build`.
6. Commit your changes with clear commit messages.
7. Open a pull request against `main` and describe what your change does.

## Code style

* **C#**: The code targets C# 13 features, uses nullable reference types, top‑level statements and records.  Treat compiler warnings as errors and prefer expression‑bodied members where appropriate.  Use the `SonarAnalyzer` warnings as guidance for code quality.
* **Angular**: Use Angular standalone components.  Structure code into `features/`, `shared/`, and `state/` folders.  Use RxJS and NgRx where state management is necessary.  Ensure templates are accessible and use Tailwind CSS for styling.

## Testing

Backend projects use xUnit for unit and integration tests.  When adding new features, write tests in the corresponding `*.Tests` project.  Frontend uses Jasmine/Karma; create spec files adjacent to the components and services.

## Issues and discussions

If you find a bug or have an enhancement suggestion, please open an issue.  Use descriptive titles and include steps to reproduce.  For more involved discussions (API design, large features), consider starting a GitHub Discussion to collect feedback.

# API Common Libraries

The `common` directory contains cross‑cutting building blocks used by all modules.

* **TavernTrashers.Api.Common.Domain** – base entities (`Entity<T>`), domain events, error handling and a generic result type.
* **TavernTrashers.Api.Common.Application** – abstractions for the event bus, command/query handlers, and application services.
* **TavernTrashers.Api.Common.Infrastructure** – default implementations for infrastructure concerns such as database migrations, messaging and caching.
* **TavernTrashers.Api.Common.Presentation** – helpers for API endpoints, problem details formatting and model binding.
* **TavernTrashers.Api.Common.Testing** – base classes and utilities for writing tests against modules.
* **TavernTrashers.Api.Common.SourceGenerators** – Roslyn source generators used to reduce boilerplate.

These projects are referenced by modules to ensure consistency across the API.

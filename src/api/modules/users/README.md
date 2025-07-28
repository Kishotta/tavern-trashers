# Users Module

The Users module will handle player accounts and identity management. It is in early development and integrates with Keycloak for authentication and authorization.

## Domain

Defines the `User` aggregate with properties such as id, username, email, password hash and associated roles.

## Application

The application layer will provide commands and queries for user registration, login, profile editing and role assignments. A domain event is raised when a user registers.

## Infrastructure

This layer integrates with external identity providers and persistent storage. Currently the project uses Postgres via Entity Framework Core for user data. Integration with Keycloak ensures OAuth2/OpenID Connect flows.

## Presentation

Endpoints will be exposed under `/api/users` for actions like:

- `POST /api/users/register` – register a new user account.
- `GET /api/users/me` – return the current authenticated user.
- `PUT /api/users/{id}` – update a user’s profile.

## Roadmap

Planned features include password reset flows, email verification, and linking accounts to characters and campaigns. Contributions are welcome!

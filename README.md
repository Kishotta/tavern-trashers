
# Tavern Trashers

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-19-DD0031?logo=angular)](https://angular.dev/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.md)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)
[![Code of Conduct](https://img.shields.io/badge/Code%20of%20Conduct-Contributor%20Covenant-4baaaa.svg)](CODE_OF_CONDUCT.md)

> A modern, enterprise-grade tabletop RPG platform built with .NET and Angular

Tavern Trashers is a modular monolith tabletop RPG platform built with .NET for the backend and Angular for the frontend. It provides a robust backend API and modern web frontends for managing campaigns, characters, dice rolls, and more.

## ✨ Features

- 🎲 **Advanced Dice Rolling**: Full support for complex dice notation (2d20+5, 4d6kh3, advantage/disadvantage)
- 👤 **Character Management**: Create and manage characters with classes, levels, and resources
- 🔐 **Secure Authentication**: OAuth 2.0 / OpenID Connect via Keycloak
- 🏗️ **Modular Architecture**: Clean, maintainable modular monolith design
- 📊 **Real-time Dashboard**: Monitor all services with .NET Aspire
- 🚀 **Production Ready**: Built with enterprise patterns and best practices

## 🏛️ Architecture

Tavern Trashers follows **Clean Architecture** and **Domain-Driven Design** principles:

- **Backend**: .NET 8 modular monolith with CQRS pattern
- **Frontend**: Angular 19 standalone components with signals
- **Database**: PostgreSQL with Entity Framework Core
- **Caching**: Redis for distributed caching
- **Messaging**: RabbitMQ for inter-module communication
- **Identity**: Keycloak for authentication and authorization
- **Gateway**: YARP reverse proxy for API routing

### Modules

- **Dice Module**: Dice rolling engine with full notation support
- **Characters Module**: Character creation and management
- **Users Module**: User authentication and authorization
- **Common Modules**: Shared domain, application, and infrastructure patterns

See [docs/ARCHITECTURE.md](./docs/ARCHITECTURE.md) for detailed architecture overview.

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- [Node.js 18+](https://nodejs.org/) and npm
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for Aspire orchestration)
- [Git](https://git-scm.com/)

### Quick Start

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Kishotta/tavern-trashers.git
   cd tavern-trashers
   ```

2. **Run with .NET Aspire (Recommended):**
   ```bash
   cd src/aspire
   dotnet run --project TavernTrashers.AppHost
   ```

   This starts the entire stack:
   - API at http://localhost:5000
   - Frontend at http://localhost:4200
   - Aspire Dashboard at http://localhost:15000
   - All infrastructure (PostgreSQL, Redis, RabbitMQ, Keycloak)

3. **Access the application:**
   - **Web App**: http://localhost:4200
   - **API Swagger**: http://localhost:5000/swagger
   - **Aspire Dashboard**: http://localhost:15000

### Alternative: Manual Setup

#### Backend

```bash
cd src
dotnet restore
dotnet build
dotnet run --project api/TavernTrashers.Api
```

#### Frontend

```bash
cd src/web/tavern-trashers-web
npm install
npm start
```

See [docs/DEVELOPMENT.md](./docs/DEVELOPMENT.md) for detailed setup instructions.

## 📚 Documentation

### User Documentation
- [User Guide](./docs/USER_GUIDE.md) - How to use Tavern Trashers
- [API Reference](./docs/API_REFERENCE.md) - Complete API documentation

### Developer Documentation
- [Development Guide](./docs/DEVELOPMENT.md) - Setup and development workflow
- [Architecture Overview](./docs/ARCHITECTURE.md) - System design and structure
- [Module Overview](./docs/MODULES.md) - Module descriptions
- [Testing Guide](./docs/TESTING.md) - Testing strategy and guidelines
- [Coding Conventions](./docs/CONVENTIONS.md) - Code standards and best practices
- [Deployment Guide](./docs/DEPLOYMENT.md) - Deployment instructions

### Module READMEs
- [Dice Module](./src/api/modules/dice/README.md)
- [Characters Module](./src/api/modules/characters/README.md)
- [Users Module](./src/api/modules/users/README.md)
- [Common Domain](./src/api/common/TavernTrashers.Api.Common.Domain/README.md)
- [Common Application](./src/api/common/TavernTrashers.Api.Common.Application/README.md)
- [Common Infrastructure](./src/api/common/TavernTrashers.Api.Common.Infrastructure/README.md)

### Project Documentation
- [Aspire App Host](./src/aspire/TavernTrashers.AppHost/README.md)
- [API Gateway](./src/api/TavernTrashers.Gateway/README.md)

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific module tests
dotnet test src/api/modules/dice/TavernTrashers.Api.Modules.Dice.Domain.Tests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Frontend tests
cd src/web/tavern-trashers-web
npm test
```

See [docs/TESTING.md](./docs/TESTING.md) for comprehensive testing guide.

## 🤝 Contributing

We welcome contributions! Please see:

- [Contributing Guidelines](./CONTRIBUTING.md) - How to contribute
- [Code of Conduct](./CODE_OF_CONDUCT.md) - Community standards
- [Coding Conventions](./docs/CONVENTIONS.md) - Code style guide

### Development Workflow

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 🔒 Security

Security is a top priority. Please see:

- [Security Policy](./SECURITY.md) - Vulnerability reporting and security practices

## 📝 License

This project is licensed under the MIT License - see [LICENSE.md](./LICENSE.md) for details.

## 🆘 Support

- 📖 [Documentation](./docs/)
- 💬 [Discussions](https://github.com/Kishotta/tavern-trashers/discussions)
- 🐛 [Issue Tracker](https://github.com/Kishotta/tavern-trashers/issues)
- 📧 [Support Guide](./SUPPORT.md)

## 🗺️ Roadmap

See [CHANGELOG.md](./CHANGELOG.md) for version history and [GitHub Issues](https://github.com/Kishotta/tavern-trashers/issues) for planned features.

### Upcoming Features

- [ ] Campaign management system
- [ ] Real-time multiplayer sessions
- [ ] Character import/export
- [ ] Mobile applications
- [ ] Dice roll macros and templates
- [ ] Character portraits and avatars
- [ ] Advanced combat tracker

## 🙏 Acknowledgments

Built with:
- [.NET](https://dotnet.microsoft.com/) - Backend framework
- [Angular](https://angular.dev/) - Frontend framework
- [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/) - Cloud-native orchestration
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - ORM
- [PostgreSQL](https://www.postgresql.org/) - Database
- [Redis](https://redis.io/) - Caching
- [RabbitMQ](https://www.rabbitmq.com/) - Message broker
- [Keycloak](https://www.keycloak.org/) - Identity management
- [YARP](https://microsoft.github.io/reverse-proxy/) - API Gateway
- [MediatR](https://github.com/jbogard/MediatR) - CQRS implementation

## 📊 Project Status

This project is in active development. Features are being added regularly, and the API may change.

---

<div align="center">

**[Website](https://taverntrashers.com)** • 
**[Documentation](./docs/)** • 
**[API Reference](./docs/API_REFERENCE.md)** • 
**[Contributing](./CONTRIBUTING.md)**

Made with ❤️ by the Tavern Trashers team

*Roll for initiative!* 🎲

</div>

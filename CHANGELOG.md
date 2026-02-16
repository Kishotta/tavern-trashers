# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Comprehensive documentation for all modules and layers
- XML documentation comments for public APIs
- Security policy and guidelines
- Support documentation

### Changed
- Enhanced README files across the codebase
- Improved architecture documentation

### Deprecated
- Nothing currently deprecated

### Removed
- Characters module (temporarily removed from current codebase)

### Fixed
- Documentation gaps in module-level READMEs

### Security
- Added SECURITY.md with vulnerability reporting process
- Documented security best practices for deployment

## [0.1.0] - 2024-01-XX

### Added
- Initial modular monolith architecture
- Backend modules: Campaigns, Dice, Users
- Angular frontend applications
- .NET Aspire orchestration
- PostgreSQL database with Entity Framework Core
- Redis caching layer
- RabbitMQ message broker
- Keycloak authentication
- YARP API Gateway
- Basic project structure and configuration

### Infrastructure
- Docker support via .NET Aspire
- Database migration service
- Development environment setup
- CI/CD workflow foundations

## Release Strategy

### Version Numbering
We follow [Semantic Versioning](https://semver.org/):
- **MAJOR**: Incompatible API changes
- **MINOR**: New functionality in a backward-compatible manner
- **PATCH**: Backward-compatible bug fixes

### Release Channels
- **Stable**: Production-ready releases
- **Beta**: Feature-complete but may contain bugs
- **Alpha**: Early preview releases

### Release Notes
Each release includes:
- New features and enhancements
- Bug fixes
- Breaking changes
- Migration guides (when applicable)
- Known issues

---

For more details, see the [releases page](https://github.com/Kishotta/tavern-trashers/releases).

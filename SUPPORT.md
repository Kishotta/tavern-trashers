# Support

Thank you for using Tavern Trashers! This document provides information on how to get help and support.

## Getting Help

### Documentation

Before seeking support, please check our documentation:

- **[README.md](./README.md)**: Project overview and quick start
- **[User Guide](./docs/USER_GUIDE.md)**: Detailed usage instructions
- **[Architecture](./docs/ARCHITECTURE.md)**: System design and structure
- **[Development Guide](./docs/DEVELOPMENT.md)**: Development setup and workflows
- **[API Reference](./docs/API_REFERENCE.md)**: API endpoint documentation
- **[FAQ](./docs/FAQ.md)**: Frequently asked questions

### Community Support

- **[GitHub Discussions](https://github.com/Kishotta/tavern-trashers/discussions)**: Ask questions, share ideas, and connect with the community
- **[Issue Tracker](https://github.com/Kishotta/tavern-trashers/issues)**: Search for existing issues or report bugs

## Reporting Issues

### Before Creating an Issue

1. **Search existing issues**: Your question may already be answered
2. **Check documentation**: Ensure you've reviewed relevant documentation
3. **Try the latest version**: Update to the latest release

### Creating a Bug Report

When reporting bugs, please include:

- **Environment**: OS, .NET version, Node.js version
- **Steps to reproduce**: Clear, step-by-step instructions
- **Expected behavior**: What should happen
- **Actual behavior**: What actually happens
- **Screenshots/logs**: Visual aids and error messages
- **Additional context**: Any other relevant information

Use our [bug report template](.github/ISSUE_TEMPLATE_bug_report.md).

### Requesting Features

For feature requests, please include:

- **Use case**: Why is this feature needed?
- **Proposed solution**: How should it work?
- **Alternatives**: Other approaches you've considered
- **Additional context**: Examples, mockups, or references

Use our [feature request template](.github/ISSUE_TEMPLATE_feature_request.md).

## Contributing

We welcome contributions! Please see:

- **[CONTRIBUTING.md](./CONTRIBUTING.md)**: Contribution guidelines
- **[CODE_OF_CONDUCT.md](./CODE_OF_CONDUCT.md)**: Community standards

## Commercial Support

Currently, this is an open-source project without commercial support options. For enterprise inquiries, please contact the maintainers through GitHub.

## Priority Support

We prioritize:

1. **Security vulnerabilities**: See [SECURITY.md](./SECURITY.md)
2. **Critical bugs**: Issues affecting core functionality
3. **Documentation improvements**: Helping users succeed
4. **Feature requests**: Community-voted enhancements

## Response Times

This is a community-driven project. Response times vary based on:

- Maintainer availability
- Issue complexity and priority
- Community involvement

**Typical response times:**
- Security issues: Within 48 hours
- Critical bugs: Within 1 week
- General questions: Best effort basis
- Feature requests: Reviewed periodically

## Self-Help Resources

### Common Issues

#### Build Failures
```bash
# Clean and restore
dotnet clean
dotnet restore
dotnet build
```

#### Database Issues
```bash
# Reset database (development only)
dotnet ef database drop --project src/api/TavernTrashers.Api
dotnet run --project src/aspire/TavernTrashers.AppHost
```

#### Frontend Issues
```bash
# Clear cache and reinstall
rm -rf node_modules package-lock.json
npm install
npm start
```

### Troubleshooting

1. **Check logs**: Review application logs for errors
2. **Verify configuration**: Ensure appsettings.json is correct
3. **Update dependencies**: Run `dotnet restore` and `npm install`
4. **Clear caches**: Delete bin/obj folders and node_modules
5. **Restart services**: Stop and restart all services

## Getting Updates

Stay informed about updates:

- **Watch the repository**: Get notified of releases
- **Star the project**: Show support and bookmark
- **Follow discussions**: Join the conversation
- **Read the changelog**: Review [CHANGELOG.md](./CHANGELOG.md)

## Code of Conduct

All support interactions must follow our [Code of Conduct](./CODE_OF_CONDUCT.md). We are committed to providing a welcoming and inclusive environment.

## Thank You

Thank you for being part of the Tavern Trashers community! Your questions, feedback, and contributions help make this project better for everyone.

---

**Need immediate help?** Start a [discussion](https://github.com/Kishotta/tavern-trashers/discussions) or search [existing issues](https://github.com/Kishotta/tavern-trashers/issues).

# Security Policy

## Supported Versions

We release patches for security vulnerabilities. Currently supported versions:

| Version | Supported          |
| ------- | ------------------ |
| 1.x.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

We take the security of Tavern Trashers seriously. If you discover a security vulnerability, please follow these steps:

### How to Report

1. **DO NOT** create a public GitHub issue for security vulnerabilities
2. Email security concerns to the maintainers at [your-email@example.com] (replace with actual contact)
3. Include the following information:
   - Description of the vulnerability
   - Steps to reproduce
   - Potential impact
   - Any suggested fixes (if available)

### What to Expect

- **Acknowledgment**: We will make our best effort to acknowledge receipt of your vulnerability report
- **Investigation**: We will investigate and validate the issue
- **Updates**: We will keep you informed of our progress
- **Resolution**: We will work to resolve critical vulnerabilities
- **Credit**: With your permission, we will credit you in the security advisory and release notes

### Security Best Practices

When deploying Tavern Trashers:

1. **Authentication**: 
   - Use strong passwords and enable multi-factor authentication
   - Configure Keycloak with appropriate security settings
   - Regularly rotate secrets and API keys

2. **Network Security**:
   - Use HTTPS/TLS for all production deployments
   - Configure firewall rules to restrict access
   - Keep infrastructure and dependencies up to date

3. **Database Security**:
   - Use strong database passwords
   - Restrict database access to application servers only
   - Regular backup and recovery testing

4. **Dependencies**:
   - Regularly update NuGet and npm packages
   - Monitor security advisories for .NET and Angular
   - Use tools like `dotnet list package --vulnerable` and `npm audit`

5. **Secrets Management**:
   - Never commit secrets to version control
   - Use environment variables or secure secret stores
   - Rotate credentials regularly

## Security Updates

Security updates will be released as patch versions and documented in [CHANGELOG.md](./CHANGELOG.md). Critical security issues will be announced through:

- GitHub Security Advisories
- Release notes
- Repository discussions

## Known Security Considerations

### Authentication & Authorization
- Keycloak is used for identity and access management
- Ensure proper role-based access control (RBAC) configuration
- Review and audit user permissions regularly

### API Security
- All API endpoints should be protected with appropriate authentication
- Rate limiting is recommended for production deployments
- Input validation is performed at multiple layers

### Data Protection
- Sensitive data should be encrypted at rest and in transit
- Follow GDPR and other applicable data protection regulations
- Implement appropriate data retention policies

## Compliance

This project aims to follow industry best practices including:
- OWASP Top 10 security risks mitigation
- Secure coding guidelines for .NET and TypeScript
- Regular security audits and dependency scanning

## Contact

For any security-related questions or concerns, please contact the maintainers through the appropriate channels listed above.

---

Thank you for helping keep Tavern Trashers and our users safe!

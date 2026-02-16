# Deployment Guide

This guide provides instructions for deploying Tavern Trashers to various environments.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Environment Configuration](#environment-configuration)
- [Local Deployment](#local-deployment)
- [Docker Deployment](#docker-deployment)
- [Azure Deployment](#azure-deployment)
- [Production Considerations](#production-considerations)
- [Monitoring](#monitoring)

## Prerequisites

### General Requirements
- .NET 8.0 SDK or later
- Node.js 18+ and npm
- Docker (for containerized deployments)
- Azure CLI (for Azure deployments)

### Infrastructure Requirements
- PostgreSQL 14+
- Redis 6+
- RabbitMQ 3.11+
- Keycloak 22+

## Environment Configuration

### Environment Variables

Create environment-specific configuration files:

#### Production (appsettings.Production.json)
```json
{
  "ConnectionStrings": {
    "Database": "Server=prod-db.example.com;Port=5432;Database=taverntrashers;User Id=${DB_USER};Password=${DB_PASSWORD};SslMode=Require;",
    "Redis": "${REDIS_CONNECTION_STRING}",
    "RabbitMQ": "${RABBITMQ_CONNECTION_STRING}"
  },
  "Keycloak": {
    "Authority": "https://auth.taverntrashers.com/realms/tavern-trashers",
    "Audience": "tavern-trashers-api",
    "RequireHttpsMetadata": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "taverntrashers.com,*.taverntrashers.com"
}
```

### Secrets Management

**Never commit secrets to source control.** Use:

- **Azure Key Vault**: For Azure deployments
- **Environment Variables**: For container deployments
- **User Secrets**: For local development
- **.env files** (in .gitignore): For Docker Compose

## Local Deployment

### Using .NET Aspire (Recommended)

```bash
cd src/aspire
dotnet run --project TavernTrashers.AppHost
```

Access the application:
- API: http://localhost:5000
- Frontend: http://localhost:4200
- Aspire Dashboard: http://localhost:15000

### Manual Setup

#### 1. Start Infrastructure

```bash
# PostgreSQL
docker run -d --name postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:14

# Redis
docker run -d --name redis -p 6379:6379 redis:6

# RabbitMQ
docker run -d --name rabbitmq \
  -p 5672:5672 -p 15672:15672 \
  rabbitmq:3-management

# Keycloak
docker run -d --name keycloak \
  -e KEYCLOAK_ADMIN=admin \
  -e KEYCLOAK_ADMIN_PASSWORD=admin \
  -p 8080:8080 \
  quay.io/keycloak/keycloak:22.0 start-dev
```

#### 2. Run Backend

```bash
cd src/api
dotnet run --project TavernTrashers.Api
```

#### 3. Run Frontend

```bash
cd src/web/tavern-trashers-web
npm install
npm start
```

## Docker Deployment

### Build Images

#### Backend API

```dockerfile
# Dockerfile.api
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/api/TavernTrashers.Api/TavernTrashers.Api.csproj", "api/TavernTrashers.Api/"]
RUN dotnet restore "api/TavernTrashers.Api/TavernTrashers.Api.csproj"
COPY src/ .
RUN dotnet build "api/TavernTrashers.Api/TavernTrashers.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "api/TavernTrashers.Api/TavernTrashers.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TavernTrashers.Api.dll"]
```

Build:
```bash
docker build -f Dockerfile.api -t taverntrashers-api:latest .
```

#### Frontend

```dockerfile
# Dockerfile.web
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist/tavern-trashers-web /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

Build:
```bash
cd src/web/tavern-trashers-web
docker build -f Dockerfile.web -t taverntrashers-web:latest .
```

### Docker Compose

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:14
    environment:
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - postgres-data:/var/lib/postgresql/data
    ports:
      - "5432:5432"

  redis:
    image: redis:6
    volumes:
      - redis-data:/data
    ports:
      - "6379:6379"

  rabbitmq:
    image: rabbitmq:3-management
    volumes:
      - rabbitmq-data:/var/lib/rabbitmq
    ports:
      - "5672:5672"
      - "15672:15672"

  keycloak:
    image: quay.io/keycloak/keycloak:22.0
    environment:
      KEYCLOAK_ADMIN: admin
      KEYCLOAK_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD}
    ports:
      - "8080:8080"
    command: start-dev

  api:
    image: taverntrashers-api:latest
    depends_on:
      - postgres
      - redis
      - rabbitmq
      - keycloak
    environment:
      ConnectionStrings__Database: ${DATABASE_CONNECTION}
      ConnectionStrings__Redis: redis:6379
      ConnectionStrings__RabbitMQ: amqp://rabbitmq:5672
    ports:
      - "5000:8080"

  web:
    image: taverntrashers-web:latest
    depends_on:
      - api
    ports:
      - "80:80"

volumes:
  postgres-data:
  redis-data:
  rabbitmq-data:
```

Run:
```bash
docker-compose up -d
```

## Azure Deployment

### Using Azure Developer CLI (azd)

#### 1. Initialize

```bash
azd init
```

#### 2. Provision Infrastructure

```bash
azd provision
```

This creates:
- Azure Container Apps for services
- Azure Database for PostgreSQL
- Azure Cache for Redis
- Azure Service Bus (replaces RabbitMQ)
- Azure Container Registry

#### 3. Deploy

```bash
azd deploy
```

### Manual Azure Setup

#### 1. Create Resource Group

```bash
az group create --name tavern-trashers-rg --location eastus
```

#### 2. Create PostgreSQL

```bash
az postgres flexible-server create \
  --resource-group tavern-trashers-rg \
  --name taverntrashers-db \
  --location eastus \
  --admin-user dbadmin \
  --admin-password <password> \
  --sku-name Standard_B2s
```

#### 3. Create Redis

```bash
az redis create \
  --resource-group tavern-trashers-rg \
  --name taverntrashers-redis \
  --location eastus \
  --sku Basic \
  --vm-size c0
```

#### 4. Create Container Registry

```bash
az acr create \
  --resource-group tavern-trashers-rg \
  --name taverntrashersacr \
  --sku Basic
```

#### 5. Push Images

```bash
az acr login --name taverntrashersacr
docker tag taverntrashers-api:latest taverntrashersacr.azurecr.io/api:latest
docker push taverntrashersacr.azurecr.io/api:latest
```

#### 6. Create Container App

```bash
az containerapp create \
  --name tavern-trashers-api \
  --resource-group tavern-trashers-rg \
  --environment my-environment \
  --image taverntrashersacr.azurecr.io/api:latest \
  --target-port 8080 \
  --ingress external \
  --registry-server taverntrashersacr.azurecr.io
```

## Production Considerations

### Security

1. **HTTPS Only**: Enforce SSL/TLS
2. **Secrets Management**: Use Azure Key Vault or similar
3. **Network Security**: Configure firewall rules
4. **Database Encryption**: Enable at-rest and in-transit encryption
5. **Regular Updates**: Keep dependencies updated

### Performance

1. **Connection Pooling**: Already configured in EF Core
2. **Response Caching**: Enable for appropriate endpoints
3. **CDN**: Use for static frontend assets
4. **Database Indexes**: Create for frequently queried columns
5. **Horizontal Scaling**: Use multiple API instances

### Reliability

1. **Health Checks**: Implement for all services
2. **Graceful Shutdown**: Handle termination signals
3. **Circuit Breakers**: Implement for external dependencies
4. **Retry Policies**: Use Polly for transient failures
5. **Database Backups**: Automated daily backups

### Monitoring

1. **Application Insights**: Azure monitoring
2. **Structured Logging**: Use Serilog
3. **Metrics**: Export to Prometheus/Azure Monitor
4. **Alerts**: Configure for critical errors
5. **Tracing**: Enable distributed tracing

## Monitoring

### Application Insights

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString)
    .AddRedis(redisConnection)
    .AddRabbitMQ(rabbitConnection);

app.MapHealthChecks("/health");
```

### Logging

```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.ApplicationInsights"],
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "ApplicationInsights",
        "Args": {
          "telemetryConverter": "Serilog.Sinks.ApplicationInsights.TelemetryConverters.TraceTelemetryConverter, Serilog.Sinks.ApplicationInsights"
        }
      }
    ]
  }
}
```

## Rollback Strategy

1. **Keep Previous Version**: Maintain previous container image
2. **Database Migrations**: Use reversible migrations
3. **Feature Flags**: Toggle features without deployment
4. **Blue-Green Deployment**: Zero-downtime deployments

## Troubleshooting

### Database Connection Issues
- Check connection string
- Verify firewall rules
- Ensure database is running
- Check SSL requirements

### Container Startup Failures
- Review container logs
- Check environment variables
- Verify image is built correctly
- Ensure dependencies are available

## Support

For deployment issues:
- Check [GitHub Issues](https://github.com/Kishotta/tavern-trashers/issues)
- Review [SUPPORT.md](../SUPPORT.md)
- Contact maintainers

---

*Deploy with confidence!* 🚀

# Tavern Trashers App Host (.NET Aspire)

The App Host project orchestrates the entire Tavern Trashers application stack using .NET Aspire, providing service discovery, configuration, and local development orchestration.

## Overview

.NET Aspire simplifies local development by managing all services, databases, and dependencies in a single coordinated experience with a powerful dashboard for monitoring.

## What is Aspire?

.NET Aspire is an opinionated, cloud-ready stack for building observable, production-ready distributed applications. It provides:

- **Service Orchestration**: Coordinate multiple services
- **Service Discovery**: Automatic service-to-service communication
- **Observability**: Built-in telemetry, metrics, and logging
- **Dashboard**: Real-time monitoring and logs
- **Resource Management**: Databases, caches, message queues

## Services Orchestrated

### Backend Services
- **API** (`TavernTrashers.Api`): Main application API
- **Gateway** (`TavernTrashers.Gateway`): YARP API Gateway
- **Migration Service** (`TavernTrashers.MigrationService`): Database migrations

### Infrastructure
- **PostgreSQL**: Primary database with pgAdmin
- **Redis**: Distributed cache with RedisInsight
- **RabbitMQ**: Message broker with management UI
- **Keycloak**: Identity and access management

### Frontend
- **Angular App** (`tavern-trashers-web`): Web application

## Running the Stack

### Start Everything

```bash
cd src/aspire
dotnet run --project TavernTrashers.AppHost
```

This starts all services and opens the Aspire Dashboard.

### Aspire Dashboard

**URL**: http://localhost:15000

**Features**:
- View all running services
- Monitor logs in real-time
- View metrics and traces
- Inspect environment variables
- Check service health

### Service URLs

Once running, services are available at:

- **API**: http://localhost:5000
- **Gateway**: http://localhost:7000
- **Frontend**: http://localhost:4200
- **Swagger**: http://localhost:5000/swagger
- **Keycloak**: http://localhost:8080
- **RabbitMQ Management**: http://localhost:15672
- **pgAdmin**: http://localhost:5050
- **RedisInsight**: http://localhost:8001

## Project Structure

```
TavernTrashers.AppHost/
├── Program.cs              # Service orchestration configuration
├── appsettings.json        # Aspire configuration
└── Properties/
    └── launchSettings.json # Launch profiles
```

## Configuration

### Program.cs

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume();

var database = postgres.AddDatabase("taverntrashers");

var redis = builder.AddRedis("redis")
    .WithRedisInsight()
    .WithDataVolume();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithDataVolume();

var keycloak = builder.AddKeycloak("keycloak")
    .WithDataVolume();

// Backend Services
var api = builder.AddProject<Projects.TavernTrashers_Api>("api")
    .WithReference(database)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithReference(keycloak);

var gateway = builder.AddProject<Projects.TavernTrashers_Gateway>("gateway")
    .WithReference(api);

// Migration Service
builder.AddProject<Projects.TavernTrashers_MigrationService>("migrations")
    .WithReference(database);

// Frontend
builder.AddNpmApp("frontend", "../web/tavern-trashers-web")
    .WithReference(gateway)
    .WithHttpEndpoint(port: 4200);

builder.Build().Run();
```

## Features

### Service References

Services can reference each other:
```csharp
var api = builder.AddProject<Projects.TavernTrashers_Api>("api")
    .WithReference(database);  // Automatically configures connection string
```

### Data Volumes

Persistent storage for data:
```csharp
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();  // Data survives restarts
```

### Wait For Dependencies

Ensure services start in order:
```csharp
var migrationService = builder.AddProject<Projects.MigrationService>("migrations")
    .WithReference(database)
    .WaitFor(database);  // Wait for database to be ready
```

### Health Checks

Automatic health monitoring:
```csharp
builder.AddHealthChecks()
    .AddCheck("database")
    .AddCheck("redis")
    .AddCheck("rabbitmq");
```

## Benefits

### Development Experience

1. **Single Command**: Start entire stack with one command
2. **Live Reload**: Services auto-restart on code changes
3. **Centralized Logs**: View all logs in one place
4. **Service Discovery**: Automatic URL configuration
5. **No Docker Compose**: Simpler than maintaining docker-compose.yml

### Observability

1. **Distributed Tracing**: W3C Trace Context
2. **Metrics**: Prometheus-compatible metrics
3. **Logging**: Structured logging with correlation IDs
4. **Dashboard**: Real-time monitoring

### Production-Ready

1. **Configuration**: Environment-specific settings
2. **Secrets**: Secure secret management
3. **Scaling**: Easy to scale services
4. **Cloud Deployment**: Deploy to Azure Container Apps

## Environment Variables

Aspire automatically configures:

```bash
# Database
ConnectionStrings__Database="Server=localhost;Port=5432;Database=taverntrashers;..."

# Redis
ConnectionStrings__Redis="localhost:6379"

# RabbitMQ
ConnectionStrings__RabbitMQ="amqp://localhost:5672"

# Keycloak
Keycloak__Authority="http://localhost:8080/realms/tavern-trashers"
```

## Troubleshooting

### Port Conflicts

If ports are already in use:
```json
{
  "Aspire": {
    "Dashboard": {
      "Port": 15000  // Change if needed
    }
  }
}
```

### Service Won't Start

1. Check Aspire Dashboard for errors
2. View service logs in dashboard
3. Ensure all dependencies are installed
4. Check firewall settings

### Database Issues

```bash
# Reset database
docker volume rm aspire-postgres-data
dotnet run --project TavernTrashers.AppHost
```

## Deployment

### Azure Container Apps

```bash
azd init
azd up
```

Aspire automatically:
- Provisions infrastructure
- Deploys services
- Configures networking
- Sets up monitoring

## Performance

### Resource Usage

Typical memory usage:
- Aspire Dashboard: ~100MB
- PostgreSQL: ~50MB
- Redis: ~10MB
- RabbitMQ: ~100MB
- Keycloak: ~200MB
- API: ~100MB
- Frontend: ~50MB

**Total**: ~600MB for full stack

### Optimization

1. **DbContext Pooling**: Enabled by default
2. **Connection Pooling**: Configured automatically
3. **Response Caching**: Available in API
4. **Distributed Caching**: Redis pre-configured

## Best Practices

1. **Use .WithDataVolume()**: For persistence
2. **Add Health Checks**: Monitor service health
3. **Use Service References**: Automatic configuration
4. **Monitor Dashboard**: Watch for issues
5. **Environment-Specific Settings**: Use appsettings.{env}.json

## Related Documentation

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Development Guide](../../../docs/DEVELOPMENT.md)
- [Deployment Guide](../../../docs/DEPLOYMENT.md)

## Contributing

When modifying the App Host:

1. Test locally before committing
2. Document new services
3. Update port mappings
4. Consider resource limits
5. Update deployment scripts

---

*Orchestration made simple!* 🎵

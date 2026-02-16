# Tavern Trashers API Gateway

The API Gateway provides a unified entry point to all backend services using YARP (Yet Another Reverse Proxy), offering routing, load balancing, and request aggregation.

## Overview

The Gateway sits between clients and backend services, routing requests to appropriate modules while providing cross-cutting concerns like authentication, rate limiting, and request transformation.

## Features

- **Reverse Proxy**: Route requests to backend services
- **Service Discovery**: Automatic route configuration
- **Load Balancing**: Distribute requests across instances
- **Authentication**: Centralized JWT validation
- **Rate Limiting**: Protect services from abuse
- **Request/Response Transformation**: Modify in-flight requests
- **Health Checks**: Monitor backend health

## Architecture

```
Client (Browser/Mobile)
    ↓
API Gateway (YARP)
    ↓
┌───────────┬───────────┬───────────┐
│  Dice     │  Users    │ Characters│
│  Module   │  Module   │  Module   │
└───────────┴───────────┴───────────┘
```

## Configuration

### Routes

Routes defined in `appsettings.json`:

```json
{
  "ReverseProxy": {
    "Routes": {
      "dice-route": {
        "ClusterId": "dice-cluster",
        "Match": {
          "Path": "/api/dice/{**catch-all}"
        },
        "Transforms": [
          {
            "PathPattern": "/api/dice/{**catch-all}"
          }
        ]
      },
      "users-route": {
        "ClusterId": "users-cluster",
        "Match": {
          "Path": "/api/users/{**catch-all}"
        }
      },
      "characters-route": {
        "ClusterId": "characters-cluster",
        "Match": {
          "Path": "/api/characters/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "dice-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "http://localhost:5001"
          }
        }
      },
      "users-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "http://localhost:5002"
          }
        }
      },
      "characters-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "http://localhost:5003"
          }
        }
      }
    }
  }
}
```

## Key Concepts

### Routes

Define URL patterns and how to match them:
```json
{
  "Match": {
    "Path": "/api/dice/{**catch-all}",
    "Methods": ["GET", "POST"]
  }
}
```

### Clusters

Define backend destinations:
```json
{
  "Clusters": {
    "dice-cluster": {
      "LoadBalancingPolicy": "RoundRobin",
      "Destinations": {
        "instance1": {
          "Address": "http://localhost:5001",
          "Health": "http://localhost:5001/health"
        },
        "instance2": {
          "Address": "http://localhost:5002",
          "Health": "http://localhost:5002/health"
        }
      }
    }
  }
}
```

### Transforms

Modify requests/responses:
```json
{
  "Transforms": [
    {
      "RequestHeader": "X-Forwarded-For",
      "Set": "{RemoteIpAddress}"
    },
    {
      "ResponseHeader": "X-Gateway-Version",
      "Set": "1.0"
    }
  ]
}
```

## Load Balancing

Available policies:
- **RoundRobin**: Distribute evenly (default)
- **LeastRequests**: Send to least busy
- **Random**: Random selection
- **PowerOfTwoChoices**: Pick best of two random

```json
{
  "LoadBalancingPolicy": "LeastRequests"
}
```

## Health Checks

Monitor backend health:

```json
{
  "HealthCheck": {
    "Active": {
      "Enabled": true,
      "Interval": "00:00:10",
      "Timeout": "00:00:05",
      "Policy": "ConsecutiveFailures",
      "Path": "/health"
    }
  }
}
```

## Authentication

Centralized JWT validation:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/tavern-trashers";
        options.Audience = "tavern-trashers-api";
    });
```

## Rate Limiting

Protect backend services:

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
    });
});
```

Apply to routes:
```json
{
  "RateLimiterPolicy": "fixed"
}
```

## CORS

Configure CORS for web clients:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
```

## Request Aggregation

Combine multiple backend calls:

```csharp
app.MapGet("/api/dashboard", async (HttpContext context) =>
{
    // Call multiple services
    var rollsTask = httpClient.GetAsync("/api/dice/rolls");
    var charactersTask = httpClient.GetAsync("/api/characters");
    
    await Task.WhenAll(rollsTask, charactersTask);
    
    // Combine results
    var dashboard = new {
        Rolls = await rollsTask.Result.Content.ReadFromJsonAsync<RollsResponse>(),
        Characters = await charactersTask.Result.Content.ReadFromJsonAsync<CharactersResponse>()
    };
    
    return Results.Ok(dashboard);
});
```

## Observability

### Logging

All requests logged:
```csharp
app.UseHttpLogging();
```

### Metrics

YARP exposes metrics:
- Request count
- Request duration
- Error rate
- Active requests

### Tracing

Distributed tracing with W3C Trace Context:
```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation());
```

## Security

### Request Validation

```csharp
// Validate request size
options.MaxRequestBodySize = 10_000_000; // 10MB

// Validate headers
options.RequestHeaderLimits.Add("Content-Length", 10_000_000);
```

### Header Management

```csharp
// Remove sensitive headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Remove("Server");
    context.Response.Headers.Remove("X-Powered-By");
    await next();
});
```

## Testing

### Test Gateway Routing

```bash
# Test dice route
curl http://localhost:7000/api/dice/rolls

# Test users route
curl http://localhost:7000/api/users/me

# Test health
curl http://localhost:7000/health
```

### Load Testing

```bash
# Using Apache Bench
ab -n 1000 -c 10 http://localhost:7000/api/dice/rolls

# Using wrk
wrk -t12 -c400 -d30s http://localhost:7000/api/dice/rolls
```

## Performance

### Optimizations

1. **Connection Pooling**: Reuse connections
2. **Response Buffering**: Optional for large responses
3. **Compression**: Gzip/Brotli for responses
4. **Caching**: Cache responses when appropriate

```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});
```

## Deployment

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "TavernTrashers.Gateway.dll"]
```

### Kubernetes

```yaml
apiVersion: v1
kind: Service
metadata:
  name: gateway
spec:
  selector:
    app: gateway
  ports:
    - protocol: TCP
      port: 80
      targetPort: 8080
```

## Troubleshooting

### Route Not Working

1. Check route configuration in appsettings.json
2. Verify backend service is running
3. Check YARP logs for errors
4. Test backend directly

### Performance Issues

1. Enable response caching
2. Check load balancing policy
3. Monitor backend health
4. Review connection limits

## Best Practices

1. **Use Health Checks**: Monitor backend health
2. **Enable Rate Limiting**: Protect services
3. **Configure Timeouts**: Prevent hanging requests
4. **Use Load Balancing**: Distribute load evenly
5. **Monitor Metrics**: Track performance
6. **Secure Headers**: Remove sensitive information

## Related Documentation

- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [API Reference](../../../docs/API_REFERENCE.md)
- [Deployment Guide](../../../docs/DEPLOYMENT.md)

---

*Your gateway to greatness!* 🚪

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
   .WithEnvironment("COMPOSE_PROJECT_NAME", "tavern-trashers")
   .WithEndpoint(5432, 5432, "tcp", "postgres", isProxied: false)
   .WithDataVolume()
   .WithPgAdmin()
   .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("database", "tavern_trashers");

var cache = builder.AddRedis("cache")
   .WithEnvironment("COMPOSE_PROJECT_NAME", "tavern-trashers")
   .WithoutHttpsCertificate()
   .WithRedisInsight()
   .WithDataVolume()
   .WithLifetime(ContainerLifetime.Persistent);

var queue = builder.AddRabbitMQ("queue")
   .WithEnvironment("COMPOSE_PROJECT_NAME", "tavern-trashers")
   .WithManagementPlugin()
   .WithDataVolume()
   .WithLifetime(ContainerLifetime.Persistent);

var identity = builder.AddKeycloak("identity", 3000)
   .WithEnvironment("COMPOSE_PROJECT_NAME", "tavern-trashers")
   .WithoutHttpsCertificate()
   .WithDataVolume()
   .WithLifetime(ContainerLifetime.Persistent)
   .WithEnvironment("PROXY_ADDRESS_FORWARDING", "true")
   .WithExternalHttpEndpoints();

var migrations = builder.AddProject<TavernTrashers_MigrationService>("migrations")
   .WithEnvironment("COMPOSE_PROJECT_NAME", "tavern-trashers")
   .WithReference(database)
   .WaitFor(database)
   .WithReference(cache)
   .WaitFor(cache)
   .WithReference(queue);

var api = builder.AddProject<TavernTrashers_Api>("api")
   .WithEnvironment("COMPOSE_PROJECT_NAME", "tavern-trashers")
   .WithReference(database)
   .WaitFor(database)
   .WaitForCompletion(migrations)
   .WithReference(cache)
   .WithReference(queue)
   .WithReference(identity)
   .WaitFor(identity)
   .WithExternalHttpEndpoints();

var gateway = builder.AddProject<TavernTrashers_Gateway>("gateway")
   .WithEnvironment("COMPOSE_PROJECT_NAME", "tavern-trashers")
   .WithReference(api)
   .WaitFor(api)
   .WithReference(identity)
   .WaitFor(identity)
   .WithExternalHttpEndpoints();

builder.AddNpmApp("web", "../../web/tavern-trashers-web")
   .WithEnvironment("COMPOSE_PROJECT_NAME", "tavern-trashers")
   .WithReference(gateway)
   .WaitFor(gateway)
   .WithReference(identity)
   .WaitFor(identity)
   .WithHttpEndpoint(env: "PORT", port: 4200)
   .WithExternalHttpEndpoints()
   .PublishAsDockerFile();

await builder.Build().RunAsync();
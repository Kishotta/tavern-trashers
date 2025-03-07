var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
   .WithEndpoint(5432, 5432, "tcp", "postgres", isProxied: false)
   .WithDataVolume()
   .WithPgAdmin()
   .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("database", "tavern_trashers");

var cache = builder.AddRedis("cache")
   .WithRedisInsight()
   .WithDataVolume()
   .WithLifetime(ContainerLifetime.Persistent);

var queue = builder.AddRabbitMQ("queue")
   .WithManagementPlugin()
   .WithDataVolume()
   .WithLifetime(ContainerLifetime.Persistent);

var identity = builder.AddKeycloak("identity", 3000)
   .WithDataVolume()
   .WithLifetime(ContainerLifetime.Persistent)
   .WithEnvironment("PROXY_ADDRESS_FORWARDING", "true")
   .WithExternalHttpEndpoints();

var migrations = builder.AddProject<Projects.TavernTrashers_MigrationService>("migrations")
   .WithReference(database)
   .WaitFor(database)
   .WithReference(cache)
   .WithReference(queue);

var api = builder.AddProject<Projects.TavernTrashers_Api>("api")
   .WithReference(database)
   .WaitFor(database)
   .WaitForCompletion(migrations)
   .WithReference(cache)
   .WithReference(queue)
   .WithReference(identity)
   .WaitFor(identity)
   .WithExternalHttpEndpoints();

var gateway = builder.AddProject<Projects.TavernTrashers_Gateway>("gateway")
   .WithReference(api) 
   .WaitFor(api)
   .WithReference(identity)
   .WaitFor(identity)
   .WithExternalHttpEndpoints();

builder.AddProject<Projects.TavernTrashers_Web>("web")
   .WaitFor(gateway)
   .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
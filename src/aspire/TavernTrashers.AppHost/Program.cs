var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
   .WithDataVolume()
   .WithPgAdmin()
   .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("database", "tavern_trashers");

var migrations = builder.AddProject<Projects.TavernTrashers_MigrationService>("migrations")
   .WithReference(database)
   .WaitFor(database);

var cache = builder.AddRedis("cache")
   .WithRedisInsight()
   .WithDataVolume();

var api = builder.AddProject<Projects.TavernTrashers_Api>("api")
   .WithReference(database)
   .WaitFor(database)
   .WaitForCompletion(migrations)
   .WithReference(cache)
   .WithExternalHttpEndpoints();

builder.AddNpmApp("spa", "../../web/tavern-trashers-spa")
   .WithReference(api)
   .WaitFor(api)
   .WithHttpEndpoint(4200, env: "PORT")
   .WithExternalHttpEndpoints()
   .PublishAsDockerFile();

await builder.Build().RunAsync();
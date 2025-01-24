var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
   .WithDataVolume()
   .WithPgAdmin()
   .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("database", "tavern_trashers");

var migrations = builder.AddProject<Projects.TavernTrashers_MigrationService>("migrations")
   .WithReference(database)
   .WaitFor(database);

builder.AddProject<Projects.TavernTrashers_Api>("api")
   .WithReference(database)
   .WaitFor(database)
   .WaitForCompletion(migrations);

await builder.Build().RunAsync();
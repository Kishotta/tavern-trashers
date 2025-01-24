using TavernTrashers.Api.Extensions;
using TavernTrashers.MigrationService;
using TavernTrashers.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddModules();

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
   .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var host = builder.Build();

await host.RunAsync();
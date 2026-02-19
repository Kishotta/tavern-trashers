using Scalar.AspNetCore;
using Serilog;
using TavernTrashers.Api.Common.Presentation.Endpoints;
using TavernTrashers.Api.Extensions;
using TavernTrashers.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.UseSerilog((context, services, configuration) => configuration
   .ReadFrom.Configuration(context.Configuration)
   .ReadFrom.Services(services)
   .Enrich.FromLogContext());

builder.Services.AddExceptionHandling();

builder.Host.UseSerilog((context, services, configuration) => configuration
   .ReadFrom.Configuration(context.Configuration)
   .ReadFrom.Services(services)
   .Enrich.FromLogContext());

builder.AddModules();

builder.Services.AddOpenApi();

builder.Services
   .AddCors(options =>
		options.AddDefaultPolicy(configurePolicy => configurePolicy
		   .AllowAnyMethod()
		   .AllowAnyHeader()
		   .AllowAnyOrigin()));

var app = builder.Build();

app.MapMcp();

app.UseCors();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.MapEndpoints();

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

await app.RunAsync();
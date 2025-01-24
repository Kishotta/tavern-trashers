using Scalar.AspNetCore;
using TavernTrashers.Api.Common.Presentation;
using TavernTrashers.Api.Extensions;
using TavernTrashers.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddModules();

builder.AddServiceDefaults();

builder.Services.AddExceptionHandling();

builder.Services.AddCors(options =>
	options.AddDefaultPolicy(configurePolicy => configurePolicy.AllowAnyMethod()
	   .AllowAnyHeader()
	   .AllowAnyOrigin()));

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.MapEndpoints();

app.UseHttpsRedirection();

app.UseExceptionHandler();

await app.RunAsync();
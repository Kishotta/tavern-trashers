using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using TavernTrashers.Api.Common.Application;
using TavernTrashers.Api.Common.Infrastructure;

namespace TavernTrashers.Api.Extensions;

public static class ModuleExtensions
{
	public static void AddModules(this IHostApplicationBuilder builder)
	{
		var modules = ModuleRepository.Modules.ToList();
		
		try
		{
			builder.AddRedisDistributedCache(connectionName: "cache");
		}
		catch
		{
			// HACK: Allows application to run without a Redis server. This is useful when, for example, generating a database migration.
			builder.Services.AddDistributedMemoryCache();
		}
		
		builder.Services
		   .ConfigureApplicationLayer(modules)
		   .ConfigureInfrastructureLayer();

		foreach (var module in modules)
		{
			module.AddModule(builder);
		}
		
		builder.Services.Configure<JsonOptions>(options =>
		{
			options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
		});
	}
}
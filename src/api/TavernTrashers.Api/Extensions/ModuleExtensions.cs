using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using TavernTrashers.Api.Common.Application;
using TavernTrashers.Api.Common.Infrastructure;
using TavernTrashers.Api.Common.Infrastructure.Caching;
using TavernTrashers.Api.Common.Infrastructure.Modules;
using TavernTrashers.Api.Common.Presentation;

namespace TavernTrashers.Api.Extensions;

public static class ModuleExtensions
{
	public static void AddModules(this IHostApplicationBuilder builder)
	{
		var modules = ModuleRepository.Modules.ToList();

		try
		{
			builder.AddDistributedCache();
		}
		catch
		{
			builder.Services.AddDistributedMemoryCache();
		}

		builder.Configuration.ConfigureModules(modules);

		builder.Services
		   .ConfigureApplicationLayer(modules)
		   .ConfigurePresentationLayer(modules)
		   .ConfigureInfrastructureLayer(builder.Configuration, modules);

		foreach (var module in modules)
			module.AddModule(builder);

		builder.Services.Configure<JsonOptions>(options =>
		{
			options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
		});
	}

	private static void ConfigureModules(this IConfigurationBuilder configurationBuilder, IEnumerable<IModule> modules)
	{
		foreach (var module in modules.Select(module => module.Name))
		{
			configurationBuilder.AddJsonFile($"modules.{module}.json", false, true);
			configurationBuilder.AddJsonFile($"modules.{module}.Development.json", false, true);
		}
	}
}
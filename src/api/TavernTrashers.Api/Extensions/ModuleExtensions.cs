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
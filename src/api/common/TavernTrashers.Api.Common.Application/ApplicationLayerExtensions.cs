using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TavernTrashers.Api.Common.Application.Behaviors;
using TavernTrashers.Api.Common.Application.Modules;

namespace TavernTrashers.Api.Common.Application;

public static class ApplicationLayerExtensions
{
	public static IServiceCollection ConfigureApplicationLayer(
		this IServiceCollection services,
		IEnumerable<IModuleApplicationLayer> modules) =>
		RegisterMediatrRequestsAndHandlers(services, modules);

	private static IServiceCollection RegisterMediatrRequestsAndHandlers(
		IServiceCollection services,
		IEnumerable<IModuleApplicationLayer> modules)
	{
		// All MediatR handlers must be registered at the same time
		var moduleApplicationAssemblies = modules
		   .Select(module => module.ApplicationAssembly)
		   .ToArray();

		services.AddMediatR(config =>
		{
			config.RegisterServicesFromAssemblies(moduleApplicationAssemblies);

			config.AddOpenBehavior(typeof(CachingPipelineBehavior<,>));
			config.AddOpenBehavior(typeof(CacheInvalidationPipelineBehavior<,>));
			config.AddOpenBehavior(typeof(ExceptionHandlingPipelineBehavior<,>));
			config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
			config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
			config.AddOpenBehavior(typeof(UnitOfWorkSaveChangesPipelineBehavior<,>));
		});

		services.AddValidatorsFromAssemblies(moduleApplicationAssemblies, includeInternalTypes: true);

		return services;
	}
}
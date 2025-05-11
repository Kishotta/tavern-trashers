using Microsoft.Extensions.DependencyInjection;
using TavernTrashers.Api.Common.Presentation.Modules;

namespace TavernTrashers.Api.Common.Presentation;

public static class PresentationLayerExtensions
{
	public static IServiceCollection ConfigurePresentationLayer(
		this IServiceCollection services,
		IEnumerable<IModulePresentationLayer> modules) =>
		RegisterModelContextProtocolTools(services, modules);

	private static IServiceCollection RegisterModelContextProtocolTools(
		IServiceCollection services,
		IEnumerable<IModulePresentationLayer> modules)
	{
		// All MCP Tools must be registered at the same time
		var modulePresentationAssemblies = modules
		   .Select(module => module.PresentationAssembly)
		   .ToArray();

		var mcpServer = services.AddMcpServer()
		   .WithHttpTransport();

		foreach (var modulePresentationAssembly in modulePresentationAssemblies)
			mcpServer.WithToolsFromAssembly(modulePresentationAssembly);

		return services;
	}
}
using Microsoft.Extensions.Hosting;
using TavernTrashers.Api.Common.Application.Modules;
using TavernTrashers.Api.Common.Presentation.Modules;

namespace TavernTrashers.Api.Common.Infrastructure.Modules;

public interface IModule : IModuleApplicationLayer, IModulePresentationLayer, IModuleInfrastructureLayer
{
	public abstract string Name { get; }
	public abstract string Schema { get; }
	
	void AddModule(IHostApplicationBuilder builder);
}
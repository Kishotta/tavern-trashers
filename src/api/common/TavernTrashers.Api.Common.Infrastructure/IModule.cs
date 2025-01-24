using System.Reflection;
using Microsoft.Extensions.Hosting;
using TavernTrashers.Api.Common.Application.Modules;
using TavernTrashers.Api.Common.Presentation;
using TavernTrashers.Api.Common.Presentation.Modules;

namespace TavernTrashers.Api.Common.Infrastructure;

public interface IModule : IApplicationModule, IPresentationModule
{
	void AddModule(IHostApplicationBuilder builder);
}

public abstract class Module : IModule
{
	public abstract Assembly ApplicationAssembly { get; }
	public abstract Assembly PresentationAssembly { get; }

	public void AddModule(IHostApplicationBuilder builder)
	{
		AddDatabase(builder);
		
		builder.Services.AddEndpoints(PresentationAssembly);
	}

	protected abstract void AddDatabase(IHostApplicationBuilder builder);
}
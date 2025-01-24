using System.Reflection;

namespace TavernTrashers.Api.Common.Application.Modules;

public interface IApplicationModule
{
	public Assembly ApplicationAssembly { get; }
}
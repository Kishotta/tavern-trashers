using System.Reflection;

namespace TavernTrashers.Api.Common.Application.Modules;

public interface IModuleApplicationLayer
{
	Assembly ApplicationAssembly { get; }
}
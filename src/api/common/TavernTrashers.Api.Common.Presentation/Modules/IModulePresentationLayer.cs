using System.Reflection;

namespace TavernTrashers.Api.Common.Presentation.Modules;

public interface IModulePresentationLayer
{
	public Assembly PresentationAssembly { get; }
}
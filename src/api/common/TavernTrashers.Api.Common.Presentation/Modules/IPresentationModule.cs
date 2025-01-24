using System.Reflection;

namespace TavernTrashers.Api.Common.Presentation.Modules;

public interface IPresentationModule
{
	public Assembly PresentationAssembly { get; }
}
using System.Reflection;

namespace TavernTrashers.Api.Modules.Characters.Infrastructure;

public static class AssemblyReference
{
	public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
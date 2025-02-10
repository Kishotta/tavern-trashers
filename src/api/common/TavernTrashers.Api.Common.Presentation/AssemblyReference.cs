using System.Reflection;

namespace TavernTrashers.Api.Common.Presentation;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
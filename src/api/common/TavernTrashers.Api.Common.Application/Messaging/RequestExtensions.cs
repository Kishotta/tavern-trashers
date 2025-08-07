using MediatR;

namespace TavernTrashers.Api.Common.Application.Messaging;

public static class RequestExtensions
{
	/// <summary>
	///     Request namespaces are structured as follows:
	///     <c>TavernTrashers.Api.Modules.[ModuleName].Application.[...]</c>
	///     The 4th element in the split array is the module name.
	/// </summary>
	private const int NamespaceModuleNamePosition = 3;

	public static string GetModuleName(this IBaseRequest request) =>
		request
		   .GetType()
		   .FullName!
		   .Split('.')[NamespaceModuleNamePosition];
}
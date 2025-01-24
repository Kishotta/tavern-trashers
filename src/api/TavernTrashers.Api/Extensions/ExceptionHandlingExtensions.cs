using TavernTrashers.Api.ExceptionHandlers;

namespace TavernTrashers.Api.Extensions;

internal static class ExceptionHandlingExtensions
{
	internal static IServiceCollection AddExceptionHandling(this IServiceCollection services) =>
		services
		   .AddExceptionHandler<GlobalExceptionHandler>()
		   .AddProblemDetails();
}
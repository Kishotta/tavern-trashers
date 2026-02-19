using MediatR;
using Microsoft.Extensions.Logging;
using TavernTrashers.Api.Common.Application.Exceptions;

namespace TavernTrashers.Api.Common.Application.Behaviors;

internal sealed partial class ExceptionHandlingPipelineBehavior<TRequest, TResponse>(ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : class
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		try
		{
			return await next(cancellationToken);
		}
		catch (Exception exception)
		{
			LogUnhandledException(typeof(TRequest).Name);

			throw new TavernTrashersException(typeof(TRequest).Name, innerException: exception);
		}
	}

	[LoggerMessage(LogLevel.Error, "Unhandled exception for {RequestName}")]
	partial void LogUnhandledException(string requestName);
}
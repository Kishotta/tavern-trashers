using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Common.Application.Behaviors;

internal sealed partial class RequestLoggingPipelineBehavior<TRequest, TResponse>(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IBaseRequest
	where TResponse : Result
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var moduleName  = request.GetModuleName();
		var requestName = typeof(TRequest).Name;

		using (LogContext.PushProperty("Module", moduleName))
		{
			LogProcessingRequest(requestName);

			var result = await next(cancellationToken);

			if (result.IsSuccess)
				LogCompletedProcessingRequest(requestName);
			else
				using (LogContext.PushProperty("Error", result.Error, true))
				{
					logger.LogError("Completed request {RequestName} with error", requestName);
				}

			return result;
		}
	}

	[LoggerMessage(LogLevel.Information, "Processing request {RequestName}")]
	partial void LogProcessingRequest(string requestName);

	[LoggerMessage(LogLevel.Information, "Processing request {RequestName}")]
	partial void LogCompletedProcessingRequest(string requestName);
}
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Common.Application.Behaviors;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
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
			logger.LogInformation("Processing request {RequestName}", requestName);

			var result = await next(cancellationToken);

			if (result.IsSuccess)
				logger.LogInformation("Completed request {RequestName}", requestName);
			else
				using (LogContext.PushProperty("Error", result.Error, true))
				{
					logger.LogError("Completed request {RequestName} with error", requestName);
				}

			return result;
		}
	}
}
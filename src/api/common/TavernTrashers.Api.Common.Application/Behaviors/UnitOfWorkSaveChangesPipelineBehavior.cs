using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Common.Application.Behaviors;

internal sealed class UnitOfWorkSaveChangesPipelineBehavior<TRequest, TResponse>(IServiceProvider serviceProvider)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IBaseCommand
	where TResponse : Result
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var response = await next(cancellationToken);

		if (response.IsFailure)
			return response;

		var moduleName = request.GetModuleName();
		var unitOfWork = serviceProvider.GetRequiredKeyedService<IUnitOfWorkBase>(moduleName);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return response;
	}
}
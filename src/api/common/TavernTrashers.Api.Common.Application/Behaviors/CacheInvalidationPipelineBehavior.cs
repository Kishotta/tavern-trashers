using MediatR;
using TavernTrashers.Api.Common.Application.Caching;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Common.Application.Behaviors;

internal sealed class CacheInvalidationPipelineBehavior<TRequest, TResponse>(ICacheService cache) 
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICacheInvalidationCommand
	where TResponse : Result
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var response = await next(cancellationToken);

		if (response.IsSuccess)
		{
			await cache.RemoveAsync(request.CacheKey, cancellationToken);
		}

		return response;
	}
}
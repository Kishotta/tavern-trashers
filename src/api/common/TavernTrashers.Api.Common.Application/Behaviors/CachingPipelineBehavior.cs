using MediatR;
using TavernTrashers.Api.Common.Application.Caching;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Common.Application.Behaviors;

internal sealed class CachingPipelineBehavior<TRequest, TResponse>(ICacheService cache) 
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICacheQuery<TResponse>
	where TResponse : Result
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var cachedValue = await cache.GetAsync<TResponse>(request.CacheKey, cancellationToken);

		if (cachedValue is not null)
			return cachedValue;
		
		var response = await next(cancellationToken);
		if (response.IsSuccess)
		{
			await cache.SetAsync(request.CacheKey, response, request.CacheDuration, cancellationToken);
		}

		return response;
	}
}
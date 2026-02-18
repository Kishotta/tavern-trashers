using MediatR;
using Microsoft.Extensions.Logging;
using TavernTrashers.Api.Common.Application.Caching;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Common.Application.Behaviors;

internal sealed class CachingPipelineBehavior<TRequest, TResponse>(
	ILogger<CachingPipelineBehavior<TRequest, TResponse>> logger,
	ICacheService cache)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICachingQuery
	where TResponse : Result
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var cacheKey = request.CacheKey;

		logger.LogInformation("Querying cache for cache key {CacheKey}", cacheKey);
		var cachedValue = await cache.GetAsync<TResponse>(cacheKey, cancellationToken);

		if (cachedValue is not null)
		{
			logger.LogInformation("Cache hit for cache key {CacheKey}", cacheKey);
			return cachedValue;
		}

		logger.LogInformation("Cache miss for cache key {CacheKey}", cacheKey);

		var response = await next(cancellationToken);
		if (response.IsSuccess)
			await cache.SetAsync(cacheKey, response, request.CacheDuration, request.CacheExpirationType, cancellationToken);

		return response;
	}
}
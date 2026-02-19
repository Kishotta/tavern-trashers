using MediatR;
using Microsoft.Extensions.Logging;
using TavernTrashers.Api.Common.Application.Caching;
using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Common.Application.Behaviors;

internal sealed partial class CachingPipelineBehavior<TRequest, TResponse>(
	ILogger<CachingPipelineBehavior<TRequest, TResponse>> logger,
	ICacheService cache)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICachingQuery
	where TResponse : Result
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var cacheKey = request.CacheKey;

		LogCacheQuery(cacheKey);
		var cachedValue = await cache.GetAsync<TResponse>(cacheKey, cancellationToken);

		if (cachedValue is not null)
		{
			LogCacheHit(cacheKey);
			return cachedValue;
		}

		LogCacheMiss(cacheKey);

		var response = await next(cancellationToken);

		if (response.IsSuccess)
			await cache.SetAsync(cacheKey, response, request.CacheDuration, request.CacheExpirationType, cancellationToken);

		return response;
	}

	[LoggerMessage(LogLevel.Trace, "Querying cache for cache key {CacheKey}")]
	partial void LogCacheQuery(string cacheKey);

	[LoggerMessage(LogLevel.Information, "Cache hit for cache key {CacheKey}")]
	partial void LogCacheHit(string cacheKey);

	[LoggerMessage(LogLevel.Trace, "Cache miss for cache key {CacheKey}")]
	partial void LogCacheMiss(string cacheKey);
}
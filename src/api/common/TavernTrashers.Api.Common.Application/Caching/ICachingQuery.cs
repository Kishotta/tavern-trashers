using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Common.Application.Caching;

public interface ICachingQuery
{
	string CacheKey { get; }
	TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
	CacheExpirationType CacheExpirationType => CacheExpirationType.Absolute;
}

public interface ICachingQuery<TResponse> : IQuery<TResponse>, ICachingQuery;
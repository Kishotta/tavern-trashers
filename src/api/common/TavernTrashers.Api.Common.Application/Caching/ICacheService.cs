using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Common.Application.Caching;

public enum CacheExpirationType
{
	Absolute,
	Sliding
}

public interface ICacheService
{
	Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

	Task SetAsync<T>(
		string key,
		T value,
		TimeSpan? expiration = null,
		CacheExpirationType expirationType = CacheExpirationType.Absolute,
		CancellationToken cancellationToken = default);

	Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}

public interface ICacheKeyProvider
{
	string CacheKey { get; }
}

public interface ICacheKeysProvider
{
	string[] CacheKeys { get; }
}

public interface ICacheDurationProvider
{
	TimeSpan CacheDuration { get; }
}

public interface ICacheExpirationTypeProvider
{
	CacheExpirationType CacheExpirationType { get; }
}

public interface ICachingQuery : ICacheKeyProvider, ICacheDurationProvider, ICacheExpirationTypeProvider;

public interface ICachingQuery<TResponse> : IQuery<TResponse>, ICachingQuery;

public interface ICacheInvalidationCommand : ICommand, ICacheKeysProvider;

public interface ICacheInvalidationCommand<TResponse> : ICommand<TResponse>, ICacheKeysProvider;
using TavernTrashers.Api.Common.Application.Messaging;

namespace TavernTrashers.Api.Common.Application.Caching;

public interface ICacheService
{
	Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

	Task SetAsync<T>(
		string key,
		T value,
		TimeSpan? expiration = null,
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

public interface ICachingQuery : ICacheKeyProvider, ICacheDurationProvider;

public interface ICachingQuery<TResponse> : IQuery<TResponse>, ICachingQuery;

public interface ICacheInvalidationCommand : ICommand, ICacheKeysProvider;

public interface ICacheInvalidationCommand<TResponse> : ICommand<TResponse>, ICacheKeysProvider;
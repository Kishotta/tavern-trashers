namespace TavernTrashers.Api.Common.Application.Caching;

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
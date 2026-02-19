using Microsoft.Extensions.Caching.Distributed;
using TavernTrashers.Api.Common.Application.Caching;

namespace TavernTrashers.Api.Common.Infrastructure.Caching;

public static class CacheOptions
{
    public static DistributedCacheEntryOptions DefaultExpiration => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
    };

    public static DistributedCacheEntryOptions Create(
        TimeSpan? expiration,
        CacheExpirationType expirationType = CacheExpirationType.Absolute)
    {
        if (expiration is null)
            return DefaultExpiration;

        return expirationType switch
        {
            CacheExpirationType.Sliding => new()
            { 
                SlidingExpiration = expiration,
            },
            CacheExpirationType.Absolute => new()
            { 
                AbsoluteExpirationRelativeToNow = expiration,
            },
            _ => DefaultExpiration,
        };
    }
}
using System;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace EcomApi.Infrastructure.Caching;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);
        return data == null ? default : JsonSerializer.Deserialize<T>(data);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task RemoveByPrefixAsync(string prefix)
    {
        await _cache.RemoveAsync($"{prefix}:all");
        for (int i = 0; i < 100; i++)
        {
            await _cache.RemoveAsync($"{prefix}:{i}");
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5),
        };
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
    }
}

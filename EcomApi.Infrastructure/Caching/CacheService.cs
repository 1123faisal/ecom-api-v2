using System.Text.Json;
using EcomApi.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace EcomApi.Infrastructure.Caching;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;

    public CacheService(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var data = await _cache.GetStringAsync(key, ct);
        return data == null ? default : JsonSerializer.Deserialize<T>(data);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(key, ct);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        // Use Redis SCAN to find all keys matching the prefix pattern, then delete them in batch.
        var db = _redis.GetDatabase();
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        const string instanceName = "EcomApi"; // must match InstanceName in AddStackExchangeRedisCache

        var pattern = $"{instanceName}{prefix}*";
        var keys = server.Keys(pattern: pattern).ToArray();
        if (keys.Length > 0)
            await db.KeyDeleteAsync(keys);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5),
        };
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options, ct);
    }
}

using System.Text.Json;
using EcomApi.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace EcomApi.Infrastructure.Caching;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<CacheService> _logger;

    public CacheService(
        IDistributedCache cache,
        IConnectionMultiplexer redis,
        ILogger<CacheService> logger
    )
    {
        _cache = cache;
        _redis = redis;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        if (!_redis.IsConnected)
            return default;
        try
        {
            var data = await _cache.GetStringAsync(key, ct);
            return data == null ? default : JsonSerializer.Deserialize<T>(data);
        }
        catch (Exception ex) when (ex is RedisConnectionException or RedisTimeoutException)
        {
            _logger.LogWarning(
                ex,
                "Cache GET failed for key '{Key}' — falling through to source.",
                key
            );
            return default;
        }
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        if (!_redis.IsConnected)
            return;
        try
        {
            await _cache.RemoveAsync(key, ct);
        }
        catch (Exception ex) when (ex is RedisConnectionException or RedisTimeoutException)
        {
            _logger.LogWarning(ex, "Cache REMOVE failed for key '{Key}'.", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        if (!_redis.IsConnected)
            return;
        try
        {
            var db = _redis.GetDatabase();
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            const string instanceName = "EcomApi";

            var pattern = $"{instanceName}{prefix}*";
            var keys = server.Keys(pattern: pattern).ToArray();
            if (keys.Length > 0)
                await db.KeyDeleteAsync(keys);
        }
        catch (Exception ex) when (ex is RedisConnectionException or RedisTimeoutException)
        {
            _logger.LogWarning(ex, "Cache REMOVE-BY-PREFIX failed for prefix '{Prefix}'.", prefix);
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiry = null,
        CancellationToken ct = default
    )
    {
        if (!_redis.IsConnected)
            return;
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5),
            };
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options, ct);
        }
        catch (Exception ex) when (ex is RedisConnectionException or RedisTimeoutException)
        {
            _logger.LogWarning(ex, "Cache SET failed for key '{Key}'.", key);
        }
    }
}

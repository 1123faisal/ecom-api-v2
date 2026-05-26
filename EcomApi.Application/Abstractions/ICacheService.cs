namespace EcomApi.Application.Abstractions;

/// <summary>
/// Abstraction over a distributed cache. Implemented by <c>CacheService</c> in the
/// Infrastructure layer; consumed by Application services to avoid a direct
/// dependency on any specific cache technology.
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default);
}

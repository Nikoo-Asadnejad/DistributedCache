namespace DistributedCache.Core.Interfaces;

public interface ICacheService
{
    Task SetAsync(string key, object data,
        TimeSpan? expirationDuration = null,
        TimeSpan? unusedExpirationDuration = null);

    Task SetListAsync(
        Dictionary<string, object> records,
        TimeSpan? expirationDuration = null,
        TimeSpan? unusedExpirationDuration = null);

    Task<T> GetAsync<T>(string key);

    Task<Dictionary<string, T>> GetListAsync<T>(List<string> keies);

    Task UpdateAsync(
        string key, object data,
        TimeSpan? expirationDuration = null,
        TimeSpan? unusedExpirationDuration = null);

    Task UpdateListAsync(
        Dictionary<string, object> keyValues,
        TimeSpan? expirationDuration = null,
        TimeSpan? unusedExpirationDuration = null);

    Task<T> GetOrSetRecordAsync<T>(
        string key, object data, Func<Task<T>> func,
        TimeSpan? expirationDuration = null,
        TimeSpan? unusedExpirationDuration = null);

    Task RemoveAsync(string key);
    Task RemoveListAsync(List<string> keies);
}
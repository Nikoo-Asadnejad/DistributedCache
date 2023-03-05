using Microsoft.Extensions.Caching.Memory;

namespace DistributedCache.Infrastructure.Services;

public class CacheService
{
    private  IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
      _memoryCache = memoryCache;
    }
    
    /// <summary>
    /// Save a record in cahce
    /// </summary>
    /// <param name="cache">IDistributedCache</param>
    /// <param name="key">A key for our data</param>
    /// <param name="data">Our data to cache</param>
    /// <param name="expirationDuration">Cache would automaticly be expired after this time</param>
    /// <param name="unusedExpirationDuration">Cache would be expired if it is not used during this time</param>
    public async Task SetAsync(string key, object data,
      TimeSpan? expirationDuration = null,
      TimeSpan? unusedExpirationDuration = null)
    {
      var options = await GenerateCacheOptions(expirationDuration, unusedExpirationDuration);
      _memoryCache.Set(key, data, options);
    }

    /// <summary>
    /// Save a List of records in redis db
    /// Save each records of dictionary as a key,value pairs in redis
    /// </summary>
    /// <param name="records">A Dictionary type which contains keies and data to cache</param>
    /// <param name="expirationDuration">Cache would automaticly be expired after this time</param>
    /// <param name="unusedExpirationDuration">Cache would be expired if it is not used during this time</param>
    public async Task SetListAsync(
      Dictionary<string, object> records,
      TimeSpan? expirationDuration = null,
      TimeSpan? unusedExpirationDuration = null)
    {
      foreach (var record in records)
      {
        await SetAsync(record.Key , record.Value ,expirationDuration , unusedExpirationDuration);
      }
    }

    /// <summary>
    /// Get the value of the key parameter
    /// </summary>
    /// <typeparam name="T">Type of data we want to retrive from db </typeparam>
    /// <param name="cache">IDistributedCache</param>
    /// <param name="key">The key of record in db</param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(
      string key)
    => _memoryCache.Get<T>(key);

    /// <summary>
    /// Get a List of Records in db which match the keis 
    /// </summary>
    /// <typeparam name="T">Type of data we want retrive from db</typeparam>
    /// <param name="cache">IDistributedCache</param>
    /// <param name="keies">List of kies we want their data</param>
    /// <returns>A Dictionary Type of kies and their value</returns>
    public async  Task<Dictionary<string, T>> GetListAsync<T>( List<string> keies)
    {
      Dictionary<string, T> result = new();
      keies.ForEach(async key => result.Add(key, await GetAsync<T>(key)));
      return result;
    }

    /// <summary>
    /// Updates the cache value
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <param name="expirationDuration"></param>
    /// <param name="unusedExpirationDuration"></param>
    /// <returns></returns>
    public async  Task UpdateAsync(
      string key, object data,
      TimeSpan? expirationDuration = null,
      TimeSpan? unusedExpirationDuration = null)
    {
      var record = _memoryCache.Get(key);
      if (record != null)
        _memoryCache.Remove(key);
      
      await SetAsync(key, data, expirationDuration, unusedExpirationDuration);
    }

    /// <summary>
    /// Updates list of  cache value
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <param name="expirationDuration"></param>
    /// <param name="unusedExpirationDuration"></param>
    /// <returns></returns>
    public async  Task UpdateListAsync(
      Dictionary<string, object> keyValues,
      TimeSpan? expirationDuration = null,
      TimeSpan? unusedExpirationDuration = null)
    {
      foreach (var keyValue in keyValues)
      {
        await UpdateAsync(keyValue.Key , keyValue.Value , expirationDuration, unusedExpirationDuration);
      }
    }

    /// <summary>
    /// Gets the data from cache if it exists ,
    /// otherwise will retrive it from the fuction and store it in the cache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <param name="func"></param>
    /// <param name="expirationDuration"></param>
    /// <param name="unusedExpirationDuration"></param>
    /// <returns></returns>
    public async  Task<T> GetOrSetRecordAsync<T>(
      string key, object data, Func<Task<T>> func,
      TimeSpan? expirationDuration = null,
      TimeSpan? unusedExpirationDuration = null)
    {
      var cachedData = await  GetAsync<T>(key);
      if (cachedData is not null)
        return cachedData;
      else
      {
        var getData = await func();
        if (getData is null)
          return default(T);

        await SetAsync(key, data ,expirationDuration, unusedExpirationDuration);
        return getData;
      }

    }
    
    private async Task<MemoryCacheEntryOptions> GenerateCacheOptions(TimeSpan? expirationDuration, TimeSpan? unusedExpirationDuration)
    {
      
        MemoryCacheEntryOptions options = new()
        {
          AbsoluteExpiration = null,
          AbsoluteExpirationRelativeToNow = expirationDuration == null ? TimeSpan.FromSeconds(60) : expirationDuration,
          Priority = CacheItemPriority.Low,
          Size = null,
          SlidingExpiration = unusedExpirationDuration
        };
        return options;
      
    }
}
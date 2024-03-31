using Newtonsoft.Json;
using StackExchange.Redis;

namespace ChargingStation.CacheManager;

public class CacheManager : ICacheManager
{
    private readonly IDatabase _database;

    public CacheManager(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }
    
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        
        if (value.IsNullOrEmpty)
        {
            return default;
        }
        
        return value.HasValue ? JsonConvert.DeserializeObject<T>(value) : default;
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        await _database.StringSetAsync(key, JsonConvert.SerializeObject(value), expiry);
    }
}
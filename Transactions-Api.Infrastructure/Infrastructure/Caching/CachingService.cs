using Microsoft.Extensions.Caching.Distributed;

namespace Transactions_Api.Infrastructure.Infrastructure.Caching;

public class CachingService : ICachingService
{
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _options;
    
    public CachingService(IDistributedCache cache)
    {
        _cache = cache;
        _options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(30)
            
        };
    }
    public async Task SetAsync(string key, string value)
    {
        await _cache.SetStringAsync(key, value, _options);
    }

    public async Task<string> GetAsync(string key)
    {
        return await _cache.GetStringAsync(key);
    }
    
    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}
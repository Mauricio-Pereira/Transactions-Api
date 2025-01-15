namespace Transactions_Api.Infrastructure.Infrastructure.Caching;

public interface ICachingService
{
    Task SetAsync(string key, string value);
    Task<string> GetAsync(string key);
    Task RemoveAsync(string txid);
}
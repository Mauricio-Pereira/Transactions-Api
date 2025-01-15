using Transactions_Api.Domain.Models;

namespace Transactions_Api.Infrastructure.Repositories
{
    public interface IApiKeyRepository
    {
        Task<ApiKey> GetByKeyAsync(string apiKey);
    }
}
using Transactions_Api.Domain.Models;

namespace Transactions_Api.Application.Services
{
    public interface IApiKeyService
    {
        Task<ApiKey> GetByKeyAsync(string apiKey);
    }
}
using Transactions_Api.Application.Services;
using Transactions_Api.Domain.Models;
using Transactions_Api.Infrastructure.Repositories;
using Transactions_Api.Shared.Exceptions;

public class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyRepository _apiKeyRepository;

    public ApiKeyService(IApiKeyRepository apiKeyRepository)
    {
        _apiKeyRepository = apiKeyRepository;
    }

    public async Task<ApiKey> GetByKeyAsync(string apiKey)
    {
        var apiKeyResult = await _apiKeyRepository.GetByKeyAsync(apiKey);
        if (apiKeyResult == null)
        {
            throw new NotFoundException("ApiKey not found");
        } 

        return apiKeyResult;
    }
}
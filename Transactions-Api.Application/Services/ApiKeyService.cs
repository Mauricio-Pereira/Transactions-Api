using Microsoft.Extensions.Logging;
using Transactions_Api.Application.Services;
using Transactions_Api.Domain.Models;
using Transactions_Api.Infrastructure.Repositories;
using Transactions_Api.Shared.Exceptions;

public class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly ILogger<ApiKeyService> _logger;

    public ApiKeyService(IApiKeyRepository apiKeyRepository, ILogger<ApiKeyService> logger)
    {
        _apiKeyRepository = apiKeyRepository;
        _logger = logger;
    }

    public async Task<ApiKey> GetByKeyAsync(string apiKey)
    {
        var apiKeyResult = await _apiKeyRepository.GetByKeyAsync(apiKey);
        if (apiKeyResult == null)
        {
            _logger.LogWarning("ApiKey not found");
            throw new NotFoundException("ApiKey not found");
        } 

        _logger.LogInformation("ApiKey found");
        return apiKeyResult;
    }
}
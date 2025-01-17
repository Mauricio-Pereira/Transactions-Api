using Transactions_Api.Domain.Models;
using Transactions_Api.Infrastructure.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Transactions_Api.Infrastructure.Repositories
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ApiKeyRepository> _logger;

        public ApiKeyRepository(AppDbContext context, ILogger<ApiKeyRepository> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<ApiKey> GetByKeyAsync(string apiKey)
        {
            _logger.LogInformation($"Getting ApiKey by key: {apiKey}");
            return await _context.ApiKeys.FirstOrDefaultAsync(a => a.Key == apiKey);
        }
    }
}
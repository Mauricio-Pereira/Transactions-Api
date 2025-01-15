using Transactions_Api.Domain.Models;
using Transactions_Api.Infrastructure.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Transactions_Api.Infrastructure.Repositories
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly AppDbContext _context;

        public ApiKeyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiKey> GetByKeyAsync(string apiKey)
        {
            return await _context.ApiKeys.FirstOrDefaultAsync(a => a.Key == apiKey);
        }
    }
}
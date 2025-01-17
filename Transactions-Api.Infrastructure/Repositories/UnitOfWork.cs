using Microsoft.Extensions.Logging;
using Transactions_Api.Infrastructure.Infrastructure;

namespace Transactions_Api.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private ITransacaoRepository? _transacaoRep;
    private IApiKeyRepository? _apiKeyRep;
    private AppDbContext _context;


    public UnitOfWork(AppDbContext context, ITransacaoRepository transacaoRepository,
        IApiKeyRepository apiKeyRepository)
    {
        _context = context;
        _transacaoRep = transacaoRepository;
        _apiKeyRep = apiKeyRepository;
    }

    public ITransacaoRepository TransacaoRepository => _transacaoRep;

    public IApiKeyRepository ApiKeyRepository => _apiKeyRep;
    
    public async Task<bool> Commit()
    {
        return await _context.SaveChangesAsync() >= 0;
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }

}
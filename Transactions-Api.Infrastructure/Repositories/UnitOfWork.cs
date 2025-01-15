using Transactions_Api.Infrastructure.Infrastructure;

namespace Transactions_Api.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private ITransacaoRepository? _transacaoRep;
    private IApiKeyRepository? _apiKeyRep;
    private AppDbContext _context;
    
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    
    
    public ITransacaoRepository TransacaoRepository
    {
        get
        {
            return _transacaoRep ??= new TransacaoRepository(_context);
        }
    }
    
    public IApiKeyRepository ApiKeyRepository
    {
        get
        {
            return _apiKeyRep ??= new ApiKeyRepository(_context);
        }
    }
    
    
    
    
    public async Task<bool> Commit()
    {
        return await _context.SaveChangesAsync() >= 0;
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }

}
using Transactions_Api.Domain.Models;
using Transactions_Api.Infrastructure.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Transactions_Api.Infrastructure.Repositories;

public class TransacaoRepository : ITransacaoRepository
{
    public AppDbContext _context;
    public TransacaoRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Transacao>> GetAllAsync()
    {
        return await _context.Transacoes.AsNoTracking().ToListAsync();
    }
    
    public async Task<IEnumerable<Transacao>> GetAllPaginatedAsync(int page, int pageSize)
    {
        return await _context.Transacoes
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }


    public async Task<Transacao> GetByTxidAsync(string txid)
    {
        var transacao = await _context.Transacoes.FirstOrDefaultAsync(x => x.Txid == txid);
        if (transacao == null)
        {
            return null;
        }
        return transacao;
    }

    public async Task<Transacao> AddAsync(Transacao transacao)
    {
        if (transacao == null) throw new ArgumentNullException(nameof(transacao));
        _context.Transacoes.AddAsync(transacao);
        _context.SaveChanges();
        return transacao;
    }

    public  async Task<Transacao> UpdateAsync(Transacao transacao)
    {
        if (transacao == null) throw new ArgumentNullException(nameof(transacao));
        _context.Entry(transacao).State = EntityState.Modified;
        _context.SaveChanges();
        return transacao;
    }

    public async Task<Transacao> DeleteAsync(string txid)
    {
        var transacaoToDelete = _context.Transacoes.FirstOrDefault(x => x.Txid == txid);
        if (transacaoToDelete == null) throw new ArgumentNullException(nameof(transacaoToDelete));
        _context.Transacoes.Remove(transacaoToDelete);
        _context.SaveChanges();
        return transacaoToDelete;

    }
    
    
    public async Task<int> CountAsync()
    {
        return await _context.Transacoes.CountAsync();
    }

}
using Transactions_Api.Domain.Models;
using Transactions_Api.Infrastructure.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Transactions_Api.Infrastructure.Repositories;

public class TransacaoRepository : ITransacaoRepository
{
    public AppDbContext _context;
    private readonly ILogger<TransacaoRepository> _logger;
    
    
    public TransacaoRepository(AppDbContext context, ILogger<TransacaoRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<IEnumerable<Transacao>> GetAllAsync()
    {
        _logger.LogInformation("Recuperando todas as transações.");
        return await _context.Transacoes.AsNoTracking().ToListAsync();
    }
    
    public async Task<IEnumerable<Transacao>> GetAllPaginatedAsync(int page, int pageSize)
    {
        _logger.LogInformation($"Recuperando transações paginadas. Página: {page}, Tamanho da página: {pageSize}");
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
            _logger.LogWarning($"Transação com Txid {txid} não encontrada.");
            return null;
        }
        _logger.LogInformation($"Transação com Txid {txid} recuperada com sucesso.");
        return transacao;
    }

    public async Task<Transacao> AddAsync(Transacao transacao)
    {
        if (transacao == null)
        {
            _logger.LogError("Tentativa de adicionar uma transação nula.");
            throw new ArgumentNullException(nameof(transacao));
        }
        _context.Transacoes.AddAsync(transacao);
        _context.SaveChanges();
        _logger.LogInformation($"Transação com Txid {transacao.Txid} adicionada com sucesso.");
        return transacao;
    }

    public  async Task<Transacao> UpdateAsync(Transacao transacao)
    {
        if (transacao == null)
        {
            _logger.LogWarning("Transação não encontrada.");
            throw new ArgumentNullException(nameof(transacao));
        }
        _context.Entry(transacao).State = EntityState.Modified;
        _context.SaveChanges();
        _logger.LogInformation($"Transação com Txid {transacao.Txid} atualizada com sucesso.");
        return transacao;
    }

    public async Task<Transacao> DeleteAsync(string txid)
    {
        var transacaoToDelete = _context.Transacoes.FirstOrDefault(x => x.Txid == txid);
        if (transacaoToDelete == null)
        {
            _logger.LogWarning($"Transação com Txid {txid} não encontrada.");
            throw new ArgumentNullException(nameof(transacaoToDelete));
        }
        _context.Transacoes.Remove(transacaoToDelete);
        _context.SaveChanges();
        _logger.LogInformation($"Transação com Txid {txid} deletada com sucesso.");
        return transacaoToDelete;

    }
    
    
    public async Task<int> CountAsync()
    {
        return await _context.Transacoes.CountAsync();
    }

}
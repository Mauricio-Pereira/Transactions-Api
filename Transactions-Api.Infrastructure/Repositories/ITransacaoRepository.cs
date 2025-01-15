using Transactions_Api.Domain.Models;

namespace Transactions_Api.Infrastructure.Repositories;

    public interface ITransacaoRepository
    {
        Task<IEnumerable<Transacao>> GetAllAsync();
        Task<IEnumerable<Transacao>> GetAllPaginatedAsync(int page, int pageSize);
        Task<Transacao> GetByTxidAsync(string txid);
        Task<Transacao> AddAsync(Transacao transacao);
        Task<Transacao> UpdateAsync(Transacao transacao);
        Task<Transacao> DeleteAsync(string  txid);
        Task<int> CountAsync();
        
    }
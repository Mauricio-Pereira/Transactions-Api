
using Transactions_Api.Application.DTOs;

namespace Transactions_Api.Application.Services
{
    public interface ITransacaoService
    {
        Task<IEnumerable<TransacaoResponseDTO>> GetAllAsync();
        Task<IEnumerable<TransacaoResponseDTO>> GetAllPaginatedAsync(int page, int pageSize);
        Task<TransacaoResponseDTO> GetByTxidAsync(string txid);
        Task<TransacaoResponseCreateDTO> AddAsync(TransacaoCreateRequestDTO transacaoRequestDto);
        Task<TransacaoResponseDTO> UpdateAsync(string txid, TransacaoUpdateDTO transacaoDto);
        Task<TransacaoResponseDTO> DeleteAsync(string txid);
    }

}
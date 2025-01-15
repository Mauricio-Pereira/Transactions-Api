namespace Transactions_Api.Application.DTOs;

public class TransacaoResourceDTO
{
    public TransacaoResponseDTO Transacao { get; set; }
    public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
}
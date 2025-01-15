namespace Transactions_Api.Application.DTOs;

public class TransacaoResponseCreateDTO
{
    public int Id { get; set; }
    public string Txid { get; set; }
    public decimal Valor { get; set; }
    
    public DateTime DataTransacao { get; set; }
}
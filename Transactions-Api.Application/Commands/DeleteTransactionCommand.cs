using Transactions_Api.Application.DTOs;
using MediatR;

namespace Transactions_Api.Application.Commands;

public class DeleteTransactionCommand : IRequest<TransacaoResponseDTO>
{
    public string Txid { get; set; }
}
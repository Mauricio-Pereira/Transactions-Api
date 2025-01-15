using Transactions_Api.Application.DTOs;
using MediatR;

namespace Transactions_Api.Application.Commands;

public class UpdateTransactionCommand : IRequest<TransacaoResourceDTO>
{
    public string Txid { get; set; }
    public TransacaoUpdateDTO UpdateDto { get; set; }
}
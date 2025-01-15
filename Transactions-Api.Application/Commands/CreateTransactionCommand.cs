using MediatR;
using Transactions_Api.Application.DTOs;

namespace Transactions_Api.Application.Commands;


public class CreateTransactionCommand : IRequest<TransacaoResponseCreateDTO>
{
    public decimal Valor { get; set; }
}
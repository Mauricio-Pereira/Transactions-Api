using Transactions_Api.Application.DTOs;
using MediatR;

namespace Transactions_Api.Application.Queries;

public class GetAllTransacoesQuery : IRequest<IEnumerable<TransacaoResourceDTO>>
{
}
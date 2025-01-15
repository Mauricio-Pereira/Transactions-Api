using Transactions_Api.Application.DTOs;
using MediatR;

namespace Transactions_Api.Application.Queries;

public class GetTransacoesPagedQuery : IRequest<(IEnumerable<TransacaoResourceDTO> Items, bool NotFound)>
{
    public int Page { get; }
    public int PageSize { get; }

    public GetTransacoesPagedQuery(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
}
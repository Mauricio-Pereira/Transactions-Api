using Transactions_Api.Application.DTOs;
using MediatR;

namespace Transactions_Api.Application.Queries;

public class GetTransacaoByTxidQuery : IRequest<TransacaoResourceDTO>
{
    public string Txid { get; }
    
    public GetTransacaoByTxidQuery(string txid)
    {
        Txid = txid;
    }
}
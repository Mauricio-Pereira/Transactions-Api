using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Queries;
using Transactions_Api.Application.Services;
using Transactions_Api.Infrastructure.Infrastructure.Caching;
using MediatR;
using Newtonsoft.Json;

namespace Transactions_Api.Application.Handlers;

public class GetAllTransacoesHandler : IRequestHandler<GetAllTransacoesQuery, IEnumerable<TransacaoResourceDTO>>
{
    private readonly ITransacaoService _transacaoService;
    private readonly ICachingService _cachingService;

    public GetAllTransacoesHandler(
        ITransacaoService transacaoService,
        ICachingService cachingService)
    {
        _transacaoService = transacaoService;
        _cachingService = cachingService;
    }

    public async Task<IEnumerable<TransacaoResourceDTO>> Handle(
        GetAllTransacoesQuery request,
        CancellationToken cancellationToken)
    {
        // 1) Tenta obter do cache
        var transacoesCache = await _cachingService.GetAsync("transacoes");
        if (!string.IsNullOrEmpty(transacoesCache))
        {
            return JsonConvert.DeserializeObject<IEnumerable<TransacaoResourceDTO>>(transacoesCache);
        }

        // 2) Busca no banco
        var transacoes = await _transacaoService.GetAllAsync();
        if (transacoes == null || !transacoes.Any())
        {
            // Retorna lista vazia ou null — o controller decide como tratar
            return Enumerable.Empty<TransacaoResourceDTO>();
        }

        // 3) Monta os recursos
        var transacoesResource = transacoes
            .Select(t => new TransacaoResourceDTO { Transacao = t })
            .ToList(); // for materialization

        // 4) Salva no cache
        await _cachingService.SetAsync("transacoes", 
            JsonConvert.SerializeObject(transacoesResource));

        return transacoesResource;
    }
    
}
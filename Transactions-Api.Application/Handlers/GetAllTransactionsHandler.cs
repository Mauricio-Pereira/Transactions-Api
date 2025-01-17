using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Queries;
using Transactions_Api.Application.Services;
using Transactions_Api.Infrastructure.Infrastructure.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Transactions_Api.Application.Handlers;

public class GetAllTransactionsHandler : IRequestHandler<GetAllTransacoesQuery, IEnumerable<TransacaoResourceDTO>>
{
    private readonly ITransacaoService _transacaoService;
    private readonly ICachingService _cachingService;
    private readonly ILogger<GetAllTransactionsHandler> _logger;

    public GetAllTransactionsHandler(
        ITransacaoService transacaoService,
        ICachingService cachingService,
        ILogger<GetAllTransactionsHandler> logger)
    {
        _transacaoService = transacaoService;
        _cachingService = cachingService;
        _logger = logger;
    }

    public async Task<IEnumerable<TransacaoResourceDTO>> Handle(
        GetAllTransacoesQuery request,
        CancellationToken cancellationToken)
    {
        // 1) Tenta obter do cache
        var transacoesCache = await _cachingService.GetAsync("transacoes");
        if (!string.IsNullOrEmpty(transacoesCache))
        {
            _logger.LogInformation("Transações recuperadas do cache.");
            return JsonConvert.DeserializeObject<IEnumerable<TransacaoResourceDTO>>(transacoesCache);
        }

        // 2) Busca no banco
        var transacoes = await _transacaoService.GetAllAsync();
        if (transacoes == null || !transacoes.Any())
        {
            _logger.LogWarning("Nenhuma transação encontrada no banco de dados.");
            // Retorna lista vazia ou null — o controller decide como tratar
            return Enumerable.Empty<TransacaoResourceDTO>();
        }

        // 3) Monta os recursos
        var transacoesResource = transacoes
            .Select(t => new TransacaoResourceDTO { Transacao = t })
            .ToList(); // for materialization

        // 4) Salva no cache
        try
        {
            await _cachingService.SetAsync("transacoes", JsonConvert.SerializeObject(transacoesResource));
            _logger.LogInformation("Transações salvas no cache.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Falha ao salvar transações no cache: {ex.Message}");
        }

        return transacoesResource;
    }
}
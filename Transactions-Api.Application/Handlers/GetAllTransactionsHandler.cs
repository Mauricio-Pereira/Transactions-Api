using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Queries;
using Transactions_Api.Application.Services;
using Transactions_Api.Application.Services.Messaging;
using Transactions_Api.Infrastructure.Infrastructure.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Transactions_Api.Application.Handlers;

public class GetAllTransactionsHandler : IRequestHandler<GetAllTransacoesQuery, IEnumerable<TransacaoResourceDTO>>
{
    private readonly ITransacaoService _transacaoService;
    private readonly ICachingService _cachingService;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<GetAllTransactionsHandler> _logger;

    public GetAllTransactionsHandler(
        ITransacaoService transacaoService,
        ICachingService cachingService,
        IMessagePublisher messagePublisher,
        ILogger<GetAllTransactionsHandler> logger)
    {
        _transacaoService = transacaoService;
        _cachingService = cachingService;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task<IEnumerable<TransacaoResourceDTO>> Handle(
        GetAllTransacoesQuery request,
        CancellationToken cancellationToken)
    {
        var transacoesCache = await _cachingService.GetAsync("transacoes");
        if (!string.IsNullOrEmpty(transacoesCache))
        {
            _logger.LogInformation("Transações recuperadas do cache.");
            return JsonConvert.DeserializeObject<IEnumerable<TransacaoResourceDTO>>(transacoesCache);
        }

        var transacoes = await _transacaoService.GetAllAsync();
        if (transacoes == null || !transacoes.Any())
        {
            _logger.LogWarning("Nenhuma transação encontrada no banco de dados.");
            return Enumerable.Empty<TransacaoResourceDTO>();
        }

        var transacoesResource = transacoes
            .Select(t => new TransacaoResourceDTO { Transacao = t })
            .ToList();

        try
        {
            await _cachingService.SetAsync("transacoes", JsonConvert.SerializeObject(transacoesResource));
            _logger.LogInformation("Transações salvas no cache.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Falha ao salvar transações no cache: {ex.Message}");
        }

        await PublishMessageAsync(transacoesResource);

        return transacoesResource;
    }

    private async Task PublishMessageAsync(IEnumerable<TransacaoResourceDTO> transacoesResource)
    {
        try
        {
            var message = JsonConvert.SerializeObject(transacoesResource.Select(t => t.Transacao));
            await _messagePublisher.PublishAsync("transactions_queue", message);
            _logger.LogInformation("Mensagem publicada no RabbitMQ com as transações recuperadas.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem no RabbitMQ.");
        }
    }
}

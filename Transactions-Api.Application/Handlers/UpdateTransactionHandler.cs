using Transactions_Api.Application.Commands;
using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Services;
using Transactions_Api.Application.Services.Messaging;
using Transactions_Api.Infrastructure.Infrastructure.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Transactions_Api.Application.Handlers;

public class UpdateTransactionHandler : IRequestHandler<UpdateTransactionCommand, TransacaoResourceDTO>
{
    private readonly ITransacaoService _transacaoService;
    private readonly ICachingService _cachingService;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<UpdateTransactionHandler> _logger;

    public UpdateTransactionHandler(
        ITransacaoService transacaoService,
        ICachingService cachingService,
        IMessagePublisher messagePublisher,
        ILogger<UpdateTransactionHandler> logger)
    {
        _transacaoService = transacaoService;
        _cachingService = cachingService;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task<TransacaoResourceDTO> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando atualização da transação com Txid: {Txid}", request.Txid);

        var transacaoAtualizada = await _transacaoService.UpdateAsync(request.Txid, request.UpdateDto);
        if (transacaoAtualizada == null)
        {
            _logger.LogWarning("Transação com Txid {Txid} não encontrada para atualização.", request.Txid);
            return null;
        }

        _logger.LogInformation("Transação com Txid {Txid} atualizada com sucesso.", request.Txid);

        var resource = new TransacaoResourceDTO
        {
            Transacao = transacaoAtualizada
        };

        try
        {
            await _cachingService.SetAsync(request.Txid, JsonConvert.SerializeObject(resource));
            _logger.LogInformation("Transação com Txid {Txid} salva no cache.", request.Txid);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Falha ao salvar a transação com Txid {Txid} no cache: {Message}", request.Txid, ex.Message);
        }

        await PublishMessageAsync(resource);

        return resource;
    }

    private async Task PublishMessageAsync(TransacaoResourceDTO resource)
    {
        try
        {
            var message = JsonConvert.SerializeObject(resource.Transacao);
            await _messagePublisher.PublishAsync("transactions_update_queue", message);
            _logger.LogInformation("Mensagem publicada no RabbitMQ para atualização de Txid: {Txid}", resource.Transacao.Txid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem no RabbitMQ para Txid: {Txid}", resource.Transacao.Txid);
        }
    }
}

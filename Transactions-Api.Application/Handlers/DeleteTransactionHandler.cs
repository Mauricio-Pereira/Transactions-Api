using Transactions_Api.Application.Commands;
using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Services;
using Transactions_Api.Application.Services.Messaging;
using Transactions_Api.Infrastructure.Infrastructure.Caching;
using Transactions_Api.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Transactions_Api.Application.Handlers;

public class DeleteTransactionHandler : IRequestHandler<DeleteTransactionCommand, TransacaoResponseDTO>
{
    private readonly ITransacaoService _transacaoService;
    private readonly ICachingService _cachingService;
    private readonly ILogger<DeleteTransactionHandler> _logger;
    private readonly IMessagePublisher _messagePublisher;

    public DeleteTransactionHandler(
        ITransacaoService transacaoService,
        ICachingService cachingService,
        ILogger<DeleteTransactionHandler> logger,
        IMessagePublisher messagePublisher)
    {
        _transacaoService = transacaoService;
        _cachingService = cachingService;
        _logger = logger;
        _messagePublisher = messagePublisher;
    }

    public async Task<TransacaoResponseDTO> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Iniciando a exclusão da transação com Txid: {request.Txid}");

        // Obter transação
        var transaction = await _transacaoService.GetByTxidAsync(request.Txid);
        if (transaction == null)
        {
            _logger.LogWarning($"Transação com Txid {request.Txid} não encontrada.");
            throw new NotFoundException("Transação não encontrada.");
        }

        // Excluir transação
        await _transacaoService.DeleteAsync(request.Txid);

        // Remover do cache
        await RemoveFromCacheAsync(request.Txid);

        // Publicar mensagem no RabbitMQ
        await PublishMessageAsync(transaction);

        _logger.LogInformation($"Transação com Txid {request.Txid} excluída com sucesso.");
        return transaction;
    }

    private async Task RemoveFromCacheAsync(string txid)
    {
        try
        {
            await _cachingService.RemoveAsync(txid);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Erro ao remover Txid {txid} do cache: {ex.Message}");
        }
    }

    private async Task PublishMessageAsync(TransacaoResponseDTO transaction)
    {
        try
        {
            var message = $"Transação excluída: {transaction.Txid}, Valor: {transaction.Valor}";
            await _messagePublisher.PublishAsync("transactions_queue", message);
            _logger.LogInformation($"Mensagem publicada no RabbitMQ: {message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem no RabbitMQ.");
        }
    }
}

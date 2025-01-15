
using Transactions_Api.Application.Commands;
using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Services;
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

    public DeleteTransactionHandler(
        ITransacaoService transacaoService,
        ICachingService cachingService,
        ILogger<DeleteTransactionHandler> logger)
    {
        _transacaoService = transacaoService;
        _cachingService = cachingService;
        _logger = logger;
    }

    public async Task<TransacaoResponseDTO> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transacaoToDelete = await _transacaoService.GetByTxidAsync(request.Txid);
        if (transacaoToDelete == null)
        {
            throw new NotFoundException("Transação não encontrada");
        }

        await _transacaoService.DeleteAsync(request.Txid);

        try
        {
            await _cachingService.RemoveAsync(request.Txid);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Falha ao tentar remover a transação do cache: {ex.Message}");
        }

        return transacaoToDelete;
    }
}
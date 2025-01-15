using Transactions_Api.Application.Commands;
using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Services;
using Transactions_Api.Infrastructure.Infrastructure.Caching;
using Transactions_Api.Shared.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Transactions_Api.Application.Handlers;

public class UpdateTransactionHandler : IRequestHandler<UpdateTransactionCommand, TransacaoResourceDTO>
{
    private readonly ITransacaoService _transacaoService;
    private readonly ICachingService _cachingService;
    private readonly ILogger<UpdateTransactionHandler> _logger;

    public UpdateTransactionHandler(
        ITransacaoService transacaoService,
        ICachingService cachingService,
        ILogger<UpdateTransactionHandler> logger)
    {
        _transacaoService = transacaoService;
        _cachingService = cachingService;
        _logger = logger;
    }

    public async Task<TransacaoResourceDTO> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transacaoAtualizada = await _transacaoService.UpdateAsync(request.Txid, request.UpdateDto);

        var resource = new TransacaoResourceDTO
        {
            Transacao = transacaoAtualizada
        };

        try
        {
            await _cachingService.SetAsync(request.Txid, JsonConvert.SerializeObject(resource));
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Falha ao tentar armazenar no cache: {ex.Message}");
        }

        return resource;
    }
}
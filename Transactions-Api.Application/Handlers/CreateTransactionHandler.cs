using Transactions_Api.Application.Commands;
using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Services;
using Transactions_Api.Infrastructure.Infrastructure.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Transactions_Api.Application.Handlers;

public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, TransacaoResponseCreateDTO>
{
    private readonly ITransacaoService _transacaoService;
    private readonly ICachingService _cachingService;
    private readonly ILogger<CreateTransactionHandler> _logger;

    public CreateTransactionHandler(
        ITransacaoService transacaoService,
        ICachingService cachingService,
        ILogger<CreateTransactionHandler> logger)
    {
        _transacaoService = transacaoService;
        _cachingService = cachingService;
        _logger = logger;
    }

    public async Task<TransacaoResponseCreateDTO> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transacaoResponse = await _transacaoService.AddAsync(new TransacaoCreateRequestDTO
        {
            Valor = request.Valor
        });

        try
        {
            await _cachingService.SetAsync(transacaoResponse.Txid, JsonConvert.SerializeObject(transacaoResponse));
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Falha ao tentar armazenar no cache: {ex.Message}");
        }

        return transacaoResponse;
    }
}
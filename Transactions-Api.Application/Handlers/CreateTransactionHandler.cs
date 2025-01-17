using Transactions_Api.Application.Commands;
using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Transactions_Api.Application.Handlers;

public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, TransacaoResponseCreateDTO>
{
    private readonly ITransacaoService _transacaoService;
    private readonly ILogger<CreateTransactionHandler> _logger;

    public CreateTransactionHandler(
        ITransacaoService transacaoService,
        ILogger<CreateTransactionHandler> logger)
    {
        _transacaoService = transacaoService;
        _logger = logger;
    }

    public async Task<TransacaoResponseCreateDTO> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processando comando CreateTransaction. Valor: {Valor}", request.Valor);

        var result = await _transacaoService.AddAsync(new TransacaoCreateRequestDTO { Valor = request.Valor });
        _logger.LogInformation("Comando CreateTransaction processado com sucesso. Txid: {Txid}", result.Txid);

        return result;
    }
}
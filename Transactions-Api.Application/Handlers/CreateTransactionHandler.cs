using Transactions_Api.Application.Commands;
using Transactions_Api.Application.DTOs;
using Transactions_Api.Application.Services;
using Transactions_Api.Application.Services.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Transactions_Api.Application.Handlers;

public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, TransacaoResponseCreateDTO>
{
    private readonly ITransacaoService _transacaoService;
    private readonly ILogger<CreateTransactionHandler> _logger;
    private readonly IMessagePublisher _messagePublisher;

    public CreateTransactionHandler(
        ITransacaoService transacaoService,
        ILogger<CreateTransactionHandler> logger,
        IMessagePublisher messagePublisher)
    {
        _transacaoService = transacaoService;
        _logger = logger;
        _messagePublisher = messagePublisher;
    }

    public async Task<TransacaoResponseCreateDTO> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processando comando CreateTransaction. Valor: {Valor}", request.Valor);

        // Criação da transação
        var result = await _transacaoService.AddAsync(new TransacaoCreateRequestDTO { Valor = request.Valor });
        _logger.LogInformation("Comando CreateTransaction processado com sucesso. Txid: {Txid}", result.Txid);

        try
        {
            // Publicação da mensagem no RabbitMQ
            var message = $"Transação criada: {result.Txid}, Valor: {result.Valor}";
            await _messagePublisher.PublishAsync("transactions_queue", message);
            _logger.LogInformation("Mensagem publicada no RabbitMQ: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem no RabbitMQ.");
        }

        return result;
    }
}
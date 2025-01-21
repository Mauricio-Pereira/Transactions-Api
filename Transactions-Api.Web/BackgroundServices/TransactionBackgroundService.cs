using Transactions_Api.Application.Services.Messaging;

namespace Transactions_API.BackgroundServices;

public class TransactionBackgroundService : BackgroundService
{
    private readonly IMessageConsumer _messageConsumer;

    public TransactionBackgroundService(IMessageConsumer messageConsumer)
    {
        _messageConsumer = messageConsumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageConsumer.StartConsumingAsync("transactions_queue", stoppingToken);
        return Task.CompletedTask;
    }
}
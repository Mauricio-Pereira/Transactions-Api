namespace Transactions_Api.Application.Services.Messaging;

public class TransactionMessagingService
{
    private readonly IMessagePublisher _messagePublisher;

    public TransactionMessagingService(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public void PublishTransaction(string transaction)
    {
        _messagePublisher.PublishAsync("transactions_queue", transaction);
    }
}

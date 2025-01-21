namespace Transactions_Api.Application.Services.Messaging;

public interface IMessageConsumer
{
    Task StartConsumingAsync(string queueName, CancellationToken cancellationToken);
}
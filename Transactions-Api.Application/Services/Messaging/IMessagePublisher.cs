namespace Transactions_Api.Application.Services.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync(string queueName, string message);
}
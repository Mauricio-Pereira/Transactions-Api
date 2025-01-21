using Microsoft.EntityFrameworkCore.Metadata;

namespace Transactions_Api.Application.Services.Messaging;

using RabbitMQ.Client;
using System.Text;

public class RabbitMQPublisher : IMessagePublisher
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMQPublisher()
    {
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;
    }

    public async Task PublishAsync(string queueName, string message)
    {
        // Declara a fila (assíncrono)
        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // Converte a mensagem para bytes
        var body = Encoding.UTF8.GetBytes(message);

        // Publica a mensagem na fila
        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            mandatory: false,
            body: body,
            cancellationToken: CancellationToken.None);
    }



    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}

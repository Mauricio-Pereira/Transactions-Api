namespace Transactions_Api.Application.Services.Messaging;

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Configuration;

public class RabbitMQConsumer : IMessageConsumer
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMQConsumer(IConfiguration configuration)
    {
        var hostName = configuration["RabbitMQ:HostName"];
        var queueName = configuration["RabbitMQ:QueueName"];
        
        var factory = new ConnectionFactory() { HostName = hostName };
        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.QueueDeclareAsync(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        ).GetAwaiter().GetResult();
    }

    public async Task StartConsumingAsync(string queueName, CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"Mensagem recebida: {message}");

            try
            {
                // Processa a mensagem
                await ProcessMessageAsync(message);

                // Confirmação manual da mensagem
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                // Em caso de erro, é possível usar BasicNackAsync para rejeitar a mensagem
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        // Inicia o consumo
        await _channel.BasicConsumeAsync(queue: queueName,
            autoAck: false, // Habilita confirmação manual
            consumer: consumer);

        Console.WriteLine("Iniciando o consumo de mensagens...");
        await Task.Delay(Timeout.Infinite, cancellationToken); // Mantém o consumidor ativo
    }

    private Task ProcessMessageAsync(string message)
    {
        // Simula o processamento da mensagem
        Console.WriteLine($"Processando mensagem: {message}");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}
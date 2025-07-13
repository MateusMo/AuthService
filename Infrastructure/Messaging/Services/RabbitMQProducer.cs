using Infrastructure.Configuration;
using Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Infrastructure.Messaging.Services;

public class RabbitMQProducer : IMessageProducer, IDisposable
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQProducer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQProducer(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQProducer> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        var factory = new ConnectionFactory()
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public async Task PublishAsync<T>(T message, string queueName) where T : class
    {
        await PublishAsync(message, queueName, CancellationToken.None);
    }

    public async Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken) where T : class
    {
        try
        {
            // Declarar a fila (cria se não existir)
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Serializar mensagem
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            // Publicar mensagem
            _channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body
            );

            _logger.LogInformation("Mensagem publicada na fila {QueueName}: {Message}", queueName, json);
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem na fila {QueueName}", queueName);
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
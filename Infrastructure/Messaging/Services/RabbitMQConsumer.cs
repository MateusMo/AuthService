using Infrastructure.Configuration;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Infrastructure.Messaging.Services;

public class RabbitMQConsumer : BackgroundService, IMessageConsumer
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQConsumer> _logger;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQConsumer(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQConsumer> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await StartConsumingAsync(stoppingToken);
    }

    private async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        // Consumir mensagens de diferentes filas
        await ConsumeQueue(_settings.Queues.FuncionarioCreated, ProcessGerenteCreatedMessage, cancellationToken);
        await ConsumeQueue(_settings.Queues.FuncionarioUpdated, ProcessGerenteUpdatedMessage, cancellationToken);
        await ConsumeQueue(_settings.Queues.FuncionarioDeleted, ProcessGerenteDeletedMessage, cancellationToken);
        await ConsumeQueue(_settings.Queues.UserLogin, ProcessUserLoginMessage, cancellationToken);

        // Manter o consumer rodando
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000, cancellationToken);
        }
    }

    private async Task ConsumeQueue(string queueName, Func<string, Task> messageHandler, CancellationToken cancellationToken)
    {
        // Declarar a fila
        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                await messageHandler(message);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem da fila {QueueName}: {Message}", queueName, message);
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        await Task.CompletedTask;
    }

    private async Task ProcessGerenteCreatedMessage(string message)
    {
        var funcionarioMessage = JsonConvert.DeserializeObject<FuncionarioCreatedMessage>(message);
        _logger.LogInformation("Processando GerenteCreated: {FuncionarioId} - {Nome}", funcionarioMessage?.FuncionarioId, funcionarioMessage?.Nome);
        
        // Aqui você pode implementar lógica adicional
        // Por exemplo: enviar email, notificar outros sistemas, etc.
        
        await Task.CompletedTask;
    }

    private async Task ProcessGerenteUpdatedMessage(string message)
    {
        var funcionarioMessage = JsonConvert.DeserializeObject<FuncionarioUpdatedMessage>(message);
        _logger.LogInformation("Processando GerenteUpdated: {FuncionarioId} - {Nome}", funcionarioMessage?.FuncionarioId, funcionarioMessage?.Nome);
        
        await Task.CompletedTask;
    }

    private async Task ProcessGerenteDeletedMessage(string message)
    {
        var funcionarioMessage = JsonConvert.DeserializeObject<FuncionarioDeletedMessage>(message);
        _logger.LogInformation("Processando GerenteDeleted: {FuncionarioId} - {Nome}", funcionarioMessage?.FuncionarioId, funcionarioMessage?.Nome);
        
        await Task.CompletedTask;
    }

    private async Task ProcessUserLoginMessage(string message)
    {
        var loginMessage = JsonConvert.DeserializeObject<UserLoginMessage>(message);
        _logger.LogInformation("Processando UserLogin: {UserId} - {Email} - {LoginTime}", loginMessage?.UserId, loginMessage?.Email, loginMessage?.LoginTime);
        
        await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Parando RabbitMQ Consumer...");
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
namespace Infrastructure.Messaging.Interfaces;

public interface IMessageProducer
{
    Task PublishAsync<T>(T message, string queueName) where T : class;
    Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken) where T : class;
}
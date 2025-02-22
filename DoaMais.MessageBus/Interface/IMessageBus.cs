
namespace DoaMais.MessageBus.Interface
{
    public interface IMessageBus
    {
        Task PublishFanoutMessageAsync<T>(string exchangeName, string queueName, T message);
        Task ConsumeFanoutMessagesAsync<T>(string exchangeName, string queueName, Func<T, Task> messageHandler, CancellationToken cancellationToken = default);
        Task PublishDirectMessageAsync<T>(string exchangeName, string routingKey, T message);
        Task ConsumeDirectMessagesAsync<T>(string exchangeName, string queueName, string routingKey, Func<T, Task> messageHandler, CancellationToken cancellationToken = default);
    }
}

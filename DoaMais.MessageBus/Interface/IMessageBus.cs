
namespace DoaMais.MessageBus.Interface
{
    public interface IMessageBus
    {
        Task PublishMessageAsync<T>(string exchangeName, string queueName, T message);
        Task ConsumeMessagesAsync<T>(string exchangeName, string queueName, Func<T, Task> messageHandler, CancellationToken cancellationToken = default);
    }
}

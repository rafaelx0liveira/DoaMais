
namespace DoaMais.MessageBus.Interface
{
    public interface IMessageBus
    {
        Task PublishMessageAsync<T>(string queueName, T message);
        Task ConsumeMessagesAsync<T>(string queueName, Func<T, Task> messageHandler, CancellationToken cancellationToken = default);
    }
}

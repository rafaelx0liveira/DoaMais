
using DoaMais.MessageBus.Model;

namespace DoaMais.MessageBus.Interface
{
    public interface IMessageBus
    {
        Task PublishMessageAsync<T>(T message);
    }
}

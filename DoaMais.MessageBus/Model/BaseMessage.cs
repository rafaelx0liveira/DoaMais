namespace DoaMais.MessageBus.Model
{
    public class BaseMessage
    {
        public Guid MessageId { get; set; }
        public DateTime CreatedAt { get; set; }

        public BaseMessage()
        {
            MessageId = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }
    }
}

namespace DoaMais.MessageBus.Model
{
    public abstract class BaseMessage
    {
        public Guid MessageId { get; set; }
        public DateTime CreatedAt { get; set; }

        protected BaseMessage()
        {
            MessageId = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }
    }

}

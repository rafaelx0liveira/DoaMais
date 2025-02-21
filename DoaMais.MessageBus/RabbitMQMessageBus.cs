using DoaMais.MessageBus.Configuration;
using DoaMais.MessageBus.Interface;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DoaMais.MessageBus
{
    public class RabbitMQMessageBus : IMessageBus
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;

        public RabbitMQMessageBus(IOptions<RabbitMQSettings> options)
        {
            var settings = options.Value;

            _hostName = settings.HostName ?? throw new ArgumentNullException(nameof(settings.HostName), "RabbitMQ HostName is missing");
            _userName = settings.UserName ?? throw new ArgumentNullException(nameof(settings.UserName), "RabbitMQ UserName is missing");
            _password = settings.Password ?? throw new ArgumentNullException(nameof(settings.Password), "RabbitMQ Password is missing");
        }

        public async Task PublishMessageAsync<T>(string queueName, T message)
        {
            if (!await ConnectionExistsAsync())
            {
                throw new InvalidOperationException("Error connecting to RabbitMQ.");
            }

            _channel ??= await _connection!.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            byte[] body = GetMessageAsByteArray(message);

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                body: body);
        }

        public async Task ConsumeMessagesAsync<T>(string queueName, Func<T, Task> messageHandler, CancellationToken cancellationToken = default)
        {
            if (!await ConnectionExistsAsync())
            {
                throw new InvalidOperationException("Error connecting to RabbitMQ.");
            }

            _channel ??= await _connection!.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var deserializedMessage = JsonSerializer.Deserialize<T>(message);

                if (deserializedMessage != null)
                {
                    await messageHandler(deserializedMessage);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
            };

            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer
            );

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        private byte[] GetMessageAsByteArray<T>(T message)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(message, options);
            return Encoding.UTF8.GetBytes(json);
        }

        private async Task CreateConnectionAsync()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _userName,
                    Password = _password
                };

                _connection = await factory.CreateConnectionAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to RabbitMQ: {ex.Message}");
                throw;
            }
        }

        private async Task<bool> ConnectionExistsAsync()
        {
            if (_connection is { IsOpen: true }) return true;

            await CreateConnectionAsync();
            return _connection is { IsOpen: true };
        }
    }
}

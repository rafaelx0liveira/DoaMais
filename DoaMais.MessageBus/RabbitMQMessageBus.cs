using DoaMais.MessageBus.Configuration;
using DoaMais.MessageBus.Interface;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace DoaMais.MessageBus
{
    public class RabbitMQMessageBus : IMessageBus
    {
        private IConnection? _connection;
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _queueName;

        public RabbitMQMessageBus(IOptions<RabbitMQSettings> options)
        {
            var settings = options.Value;

            _hostName = settings.HostName ?? throw new ArgumentNullException(nameof(settings.HostName), "RabbitMQ HostName is missing");
            _userName = settings.UserName ?? throw new ArgumentNullException(nameof(settings.UserName), "RabbitMQ UserName is missing");
            _password = settings.Password ?? throw new ArgumentNullException(nameof(settings.Password), "RabbitMQ Password is missing");
            _queueName = settings.QueueName ?? throw new ArgumentNullException(nameof(settings.QueueName), "RabbitMQ QueueName is missing");
        }

        public async Task PublishMessageAsync<T>(T message)
        {
            if (!await ConnectionExistsAsync())
            {
                throw new InvalidOperationException("Error connecting to RabbitMQ.");
            }

            await using var channel = await _connection!.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            byte[] body = GetMessageAsByteArray(message);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _queueName,
                body: body);
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

using DoaMais.MessageBus.Configuration;
using DoaMais.MessageBus.Interface;
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

        public RabbitMQMessageBus(RabbitMQSettings settings)
        {
            if(settings == null) throw new ArgumentNullException(nameof(settings));

            _hostName = settings.HostName ?? throw new ArgumentNullException(nameof(settings.HostName), "RabbitMQ HostName is missing");
            _userName = settings.UserName ?? throw new ArgumentNullException(nameof(settings.UserName), "RabbitMQ UserName is missing");
            _password = settings.Password ?? throw new ArgumentNullException(nameof(settings.Password), "RabbitMQ Password is missing");
        }

        public async Task PublishFanoutMessageAsync<T>(string exchangeName, string queueName, T message)
        {
            if (!await ConnectionExistsAsync())
            {
                throw new InvalidOperationException("Error connecting to RabbitMQ.");
            }

            _channel ??= await _connection!.CreateChannelAsync();

            // Criar o Exchange do tipo Fanout
            await _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null);

            // Criar a fila
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Vincular a fila ao Exchange
            await _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "");

            byte[] body = GetMessageAsByteArray(message);

            await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: "",
                mandatory: false,
                body: body);
        }

        public async Task ConsumeFanoutMessagesAsync<T>(string exchangeName, string queueName, Func<T, Task> messageHandler, CancellationToken cancellationToken = default)
        {
            if (!await ConnectionExistsAsync())
            {
                throw new InvalidOperationException("Error connecting to RabbitMQ.");
            }

            _channel ??= await _connection!.CreateChannelAsync();

            // Declara o Exchange como Fanout
            await _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            // Declara a fila (caso ainda não tenha sido criada)
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            // Faz o bind da fila ao Exchange Fanout
            await _channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: "", cancellationToken: cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var deserializedMessage = JsonSerializer.Deserialize<T>(message, options);

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

        public async Task PublishDirectMessageAsync<T>(string exchangeName, string queueName, string routingKey, T message)
        {
            if (!await ConnectionExistsAsync())
            {
                throw new InvalidOperationException("Error connecting to RabbitMQ.");
            }

            _channel ??= await _connection!.CreateChannelAsync();

            // Criar Exchange Direct
            await _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                arguments: null);

            // Criar a fila ANTES de publicar a mensagem
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Vincular a fila ao Exchange com uma Routing Key específica
            await _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey);

            byte[] body = GetMessageAsByteArray(message);

            await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: routingKey, // Routing Key é importante no Direct
                mandatory: false,
                body: body);
        }

        public async Task ConsumeDirectMessagesAsync<T>(string exchangeName, string queueName, string routingKey, Func<T, Task> messageHandler, CancellationToken cancellationToken = default)
        {
            if (!await ConnectionExistsAsync())
            {
                throw new InvalidOperationException("Error connecting to RabbitMQ.");
            }

            _channel ??= await _connection!.CreateChannelAsync();

            // Criar Exchange Direct
            await _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                arguments: null);

            // Criar a fila ANTES de consumir
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Vincular a fila ao Exchange
            await _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var deserializedMessage = JsonSerializer.Deserialize<T>(message, options);


                if (deserializedMessage != null)
                {
                    await messageHandler(deserializedMessage);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
            };

            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer);

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

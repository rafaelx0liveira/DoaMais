using DoaMais.MessageBus.Configuration;
using DoaMais.MessageBus.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
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
        private readonly ILogger _logger;

        public RabbitMQMessageBus(RabbitMQSettings settings, ILogger logger)
        {
            _logger = logger;

            if(settings == null)
            {
                _logger.Error("[RabbitMQMessageBus] - HostName, UserName and Password are required. Unable to connect to RabbitMQ.");
                throw new ArgumentNullException(nameof(settings));
            }

            _hostName = settings.HostName ?? throw new ArgumentNullException(nameof(settings.HostName), "RabbitMQ HostName is missing");
            _userName = settings.UserName ?? throw new ArgumentNullException(nameof(settings.UserName), "RabbitMQ UserName is missing");
            _password = settings.Password ?? throw new ArgumentNullException(nameof(settings.Password), "RabbitMQ Password is missing");
        }

        public async Task PublishFanoutMessageAsync<T>(string exchangeName, string queueName, T message)
        {
            if (!await ConnectionExistsAsync())
            {
                _logger.Error("[RabbitMQMessageBus] - Error connecting to RabbitMQ. Connection is null.");
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

            _logger.Information($"[RabbitMQMessageBus] - Publishing message to Exchange: {exchangeName}");

            byte[] body = GetMessageAsByteArray(message);

            await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: "",
                mandatory: false,
                body: body);

            _logger.Information($"[RabbitMQMessageBus] - Message published to Exchange: {exchangeName}");
        }

        public async Task ConsumeFanoutMessagesAsync<T>(string exchangeName, string queueName, Func<T, Task> messageHandler, CancellationToken cancellationToken = default)
        {
            if (!await ConnectionExistsAsync())
            {
                _logger.Error("[RabbitMQMessageBus] - Error connecting to RabbitMQ. Connection is null.");
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

            _logger.Information($"[RabbitMQMessageBus] - Consuming messages from Exchange: {exchangeName}");

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

                _logger.Information($"[RabbitMQMessageBus] - Message consumed from Exchange: {exchangeName}");
                await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
            };

            _logger.Information($"[RabbitMQMessageBus] - Consuming messages from Queue: {queueName}");

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
                _logger.Error("[RabbitMQMessageBus] - Error connecting to RabbitMQ. Connection is null.");
                throw new InvalidOperationException("Error connecting to RabbitMQ.");
            }

            _channel ??= await _connection!.CreateChannelAsync();

            _logger.Information($"[RabbitMQMessageBus] - Publishing message to Exchange: {exchangeName}");

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

            _logger.Information($"[RabbitMQMessageBus] - Message published to Exchange: {exchangeName}");
        }

        public async Task ConsumeDirectMessagesAsync<T>(string exchangeName, string queueName, string routingKey, Func<T, Task> messageHandler, CancellationToken cancellationToken = default)
        {
            if (!await ConnectionExistsAsync())
            {
                _logger.Error("[RabbitMQMessageBus] - Error connecting to RabbitMQ. Connection is null.");
                throw new InvalidOperationException("Error connecting to RabbitMQ.");
            }

            _channel ??= await _connection!.CreateChannelAsync();

            _logger.Information($"[RabbitMQMessageBus] - Consuming messages from Exchange: {exchangeName}");

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

                _logger.Information($"[RabbitMQMessageBus] - Message consumed from Exchange: {exchangeName}");

                await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
            };

            _logger.Information($"[RabbitMQMessageBus] - Consuming messages from Queue: {queueName}");

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
                _logger.Information("[RabbitMQMessageBus] - Creating connection to RabbitMQ...");

                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _userName,
                    Password = _password
                };

                _connection = await factory.CreateConnectionAsync();

                _logger.Information("[RabbitMQMessageBus] - Connection to RabbitMQ created.");
            }
            catch (Exception ex)
            {
                _logger.Error("[RabbitMQMessageBus] - Error connecting to RabbitMQ: {Message}", ex.Message);
                throw new InvalidOperationException("Error connecting to RabbitMQ.", ex);
            }
        }

        private async Task<bool> ConnectionExistsAsync()
        {
            if (_connection is { IsOpen: true })
            {
                _logger.Information("[RabbitMQMessageBus] - Connection to RabbitMQ already exists.");
                return true;
            }

            await CreateConnectionAsync();
            return _connection is { IsOpen: true };
        }
    }
}

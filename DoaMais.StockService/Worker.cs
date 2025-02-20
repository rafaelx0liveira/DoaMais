using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;
using DoaMais.Application.DTOs;
namespace DoaMais.StockService
{
    public class StockWorker : BackgroundService
    {
        private readonly ILogger<StockWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _queueName;
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private IChannel? _channel; // Mudança para IChannel

        public StockWorker(ILogger<StockWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _queueName = _configuration["RabbitMQ:DonationQueueName"] ?? throw new ArgumentNullException("RabbitMQ queue name is missing.");

            _factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"] ?? throw new ArgumentNullException("RabbitMQ HostName is missing."),
                UserName = _configuration["RabbitMQ:UserName"] ?? throw new ArgumentNullException("RabbitMQ UserName is missing."),
                Password = _configuration["RabbitMQ:Password"] ?? throw new ArgumentNullException("RabbitMQ Password is missing."),
            };
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando StockWorker...");

            _connection = await _factory.CreateConnectionAsync(cancellationToken: cancellationToken);

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StockService rodando e ouvindo RabbitMQ...");

            if (_connection == null)
            {
                _logger.LogError("A conexão do RabbitMQ não foi inicializada.");
                return;
            }
            _channel = await _connection.CreateChannelAsync(null, stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var donationEvent = JsonSerializer.Deserialize<DonationRegisteredEvent>(message);

                if (donationEvent != null)
                {
                    await ProcessDonation(donationEvent, stoppingToken);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            };

            await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false, // Confirmação manual de mensagens
                consumer: consumer
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task ProcessDonation(DonationRegisteredEvent donationEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[StockService] Processando doação: {donationEvent.DonorId} - {donationEvent.Quantity}ml de {donationEvent.BloodType}");

            // Simulando atualização de estoque
            await Task.Delay(500, cancellationToken);

            _logger.LogInformation("[StockService] Estoque atualizado com sucesso!");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finalizando StockWorker...");

            if (_channel != null)
            {
                await _channel.CloseAsync(cancellationToken);
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync(cancellationToken);
                await _connection.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}

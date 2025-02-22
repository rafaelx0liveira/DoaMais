using DoaMais.StockService.Model;
using DoaMais.StockService.Repository.Interface;
using DoaMais.StockService.ValueObject;
using DoaMais.MessageBus.Interface;
using DoaMais.StockService.DTOs;

namespace DoaMais.StockService
{
    public class StockWorker : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StockWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _donationExchangeName;
        private readonly string _donationQueueName;

        private readonly string _lowStockRoutingKeyName;
        private readonly string _lowStockExchangeName;

        public StockWorker(
            IMessageBus messageBus,
            IServiceScopeFactory scopeFactory,
            ILogger<StockWorker> logger, 
            IConfiguration configuration)
        {
            _messageBus = messageBus;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;

            _donationExchangeName = _configuration["RabbitMQ:DonationExchangeName"] ?? throw new ArgumentNullException("DonationExchangeName not found.");
            _donationQueueName = _configuration["RabbitMQ:DonationQueueName"] ?? throw new ArgumentNullException("DonationQueueName not found.");

            _lowStockRoutingKeyName = _configuration["RabbitMQ:LowStockRoutingKeyName"] ?? throw new ArgumentNullException("LowStockRoutingKeyName not found.");
            _lowStockExchangeName = _configuration["RabbitMQ:LowStockAlertExchangeName"] ?? throw new ArgumentNullException("LowStockAlertExchangeName not found.");
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando StockWorker...");

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StockWorker rodando e ouvindo RabbitMQ...");

            await _messageBus.ConsumeFanoutMessagesAsync<DonationRegisteredEvent>(_donationExchangeName, _donationQueueName, async (donationEvent) =>
            {
                await ProcessDonation(donationEvent, stoppingToken);
            }, stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task ProcessDonation(DonationRegisteredEvent donationEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[StockService] Processando doação: {donationEvent.DonorId} - {donationEvent.Quantity}ml de {donationEvent.BloodType} - {donationEvent.RHFactor}");

            using var scope = _scopeFactory.CreateScope();
            var stockRepository = scope.ServiceProvider.GetRequiredService<IBloodStockRepository>();

            var stock = await stockRepository.GetBloodByRHAndTypeAsync(donationEvent.BloodType, donationEvent.RHFactor);
            if (stock == null)
            {
                stock = new BloodStock
                {
                    BloodType = donationEvent.BloodType,
                    QuantityML = donationEvent.Quantity
                };
                await stockRepository.AddBloodToStockAsync(stock);
            }
            else
            {
                stock.QuantityML += donationEvent.Quantity;
                await stockRepository.UpdateQuantityFromStockAsync(stock);
            }

            if (stock.QuantityML < 100) // minimum quantity in stock
            {
                var lowStockAlert = new LowStockAlertEvent(donationEvent.BloodType, donationEvent.RHFactor, stock.QuantityML);
                var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
                await messageBus.PublishDirectMessageAsync(_lowStockExchangeName, _lowStockRoutingKeyName, lowStockAlert);
            }

            _logger.LogInformation("[StockService] Estoque atualizado com sucesso!");
            _logger.LogInformation("Finalizando StockWorker...");
        }
    }

}

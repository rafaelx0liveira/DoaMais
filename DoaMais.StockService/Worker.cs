using DoaMais.StockService.Model;
using DoaMais.StockService.Repository.Interface;
using DoaMais.StockService.ValueObject;
using DoaMais.MessageBus.Interface;

namespace DoaMais.StockService
{
    public class StockWorker : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StockWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _exchangeName;
        private readonly string _queueName;

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
            _exchangeName = _configuration["RabbitMQ:ExchangeName"] ?? throw new ArgumentNullException("ExchangeName not found.");
            _queueName = _configuration["RabbitMQ:QueueName"] ?? throw new ArgumentNullException("QueueName not found.");
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando StockWorker...");

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StockWorker rodando e ouvindo RabbitMQ...");

            await _messageBus.ConsumeMessagesAsync<DonationRegisteredEvent>(_exchangeName, _queueName, async (donationEvent) =>
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

            //if (stock.QuantityML < 5) // Estoque crítico
            //{
            //    var lowStockAlert = new LowStockAlertEvent(donationEvent.BloodType, stock.Quantity);
            //    var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            //    await messageBus.PublishMessageAsync(lowStockAlert, "low_stock_alert_queue", cancellationToken);
            //}

            _logger.LogInformation("[StockService] Estoque atualizado com sucesso!");
            _logger.LogInformation("Finalizando StockWorker...");
        }
    }

}

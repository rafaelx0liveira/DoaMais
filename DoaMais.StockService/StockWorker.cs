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

        private readonly string _lowStockQueueName;
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

            _lowStockQueueName = _configuration["RabbitMQ:LowStockAlertQueueName"] ?? throw new ArgumentNullException("LowStockAlertQueueName not found.");
            _lowStockRoutingKeyName = _configuration["RabbitMQ:LowStockRoutingKeyName"] ?? throw new ArgumentNullException("LowStockRoutingKeyName not found.");
            _lowStockExchangeName = _configuration["RabbitMQ:LowStockAlertExchangeName"] ?? throw new ArgumentNullException("LowStockAlertExchangeName not found.");
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[StockService] - Iniciando...");

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[StockService] - Rodando e ouvindo RabbitMQ...");

            await _messageBus.ConsumeFanoutMessagesAsync<DonationRegisteredEventVO>(_donationExchangeName, _donationQueueName, async (donationEvent) =>
            {
                await ProcessDonation(donationEvent, stoppingToken);
            }, stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task ProcessDonation(DonationRegisteredEventVO donationEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[StockService] - Processando doação: {donationEvent.DonorId} - {donationEvent.Quantity}ml de {donationEvent.BloodType} - {donationEvent.RHFactor}");

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
                _logger.LogInformation($"[StockService] - Estoque em quantidade crítica - {stock.QuantityML} ml!");

                var adminRepository = scope.ServiceProvider.GetRequiredService<IAdminRepository>();
                List<AdminDTO> admins = await adminRepository.GetAdministratorsAsync();

                _logger.LogInformation($"[StockService] - Publicando mensagem de alerta para os administradores na fila {_lowStockQueueName}");

                var lowStockAlert = new LowStockAlertEventDTO(donationEvent.BloodType, donationEvent.RHFactor, stock.QuantityML, admins);
                var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
                await messageBus.PublishDirectMessageAsync(_lowStockExchangeName, _lowStockQueueName, _lowStockRoutingKeyName, lowStockAlert);
            }

            _logger.LogInformation("[StockService] Estoque atualizado com sucesso!");
            _logger.LogInformation("Finalizando StockWorker...");
        }
    }

}

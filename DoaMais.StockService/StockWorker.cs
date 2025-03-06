using DoaMais.StockService.Model;
using DoaMais.StockService.Repository.Interface;
using DoaMais.MessageBus.Interface;
using DoaMais.StockService.DTOs;
using DoaMais.StockService.ValueObject;

namespace DoaMais.StockService
{
    public class StockWorker : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StockWorker> _logger;
        private readonly IConfiguration _configuration;

        private readonly string _stockEventsExchangeName;

        private readonly string _donationQueueName;
        private readonly string _donationRoutingKey;

        private readonly string _transfusionQueueName;
        private readonly string _transfusionRoutingKey;

        private readonly string _lowStockQueueName;
        private readonly string _lowStockRoutingKeyName;
        private readonly string _lowStockExchangeName;

        private readonly string _donorNotificationQueueName;
        private readonly string _donorNotificationRoutingKeyName;
        private readonly string _donorNotificationExchangeName;

        private readonly string _hospitalNotificationQueueName;
        private readonly string _hospitalNotificationRoutingKeyName;
        private readonly string _hospitalNotificationExchangeName;

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

            _stockEventsExchangeName = _configuration["RabbitMQ:StockEventsExchangeName"] ?? throw new ArgumentNullException("StockEventsExchangeName not found.");

            _donationQueueName = _configuration["RabbitMQ:DonationQueueName"] ?? throw new ArgumentNullException("DonationQueueName not found.");
            _donationRoutingKey = _configuration["RabbitMQ:DonationRoutingKey"] ?? throw new ArgumentNullException("DonationRoutingKey not found.");

            _transfusionQueueName = _configuration["RabbitMQ:TransfusionQueueName"] ?? throw new ArgumentNullException("TransfusionQueueName not found.");
            _transfusionRoutingKey = _configuration["RabbitMQ:TransfusionRoutingKey"] ?? throw new ArgumentNullException("TransfusionRoutingKey not found.");

            _lowStockQueueName = _configuration["RabbitMQ:LowStockAlertQueueName"] ?? throw new ArgumentNullException("LowStockAlertQueueName not found.");
            _lowStockRoutingKeyName = _configuration["RabbitMQ:LowStockRoutingKeyName"] ?? throw new ArgumentNullException("LowStockRoutingKeyName not found.");
            _lowStockExchangeName = _configuration["RabbitMQ:LowStockAlertExchangeName"] ?? throw new ArgumentNullException("LowStockAlertExchangeName not found.");

            _donorNotificationQueueName = _configuration["RabbitMQ:DonorNotificationQueueName"] ?? throw new ArgumentNullException("DonorNotificationQueueName not found.");
            _donorNotificationRoutingKeyName = _configuration["RabbitMQ:DonorNotificationRoutingKey"] ?? throw new ArgumentNullException("DonorNotificationRoutingKey not found.");
            _donorNotificationExchangeName = _configuration["RabbitMQ:DonorNotificationExchangeName"] ?? throw new ArgumentNullException("DonorNotificationExchangeName not found.");

            _hospitalNotificationQueueName = _configuration["RabbitMQ:HospitalNotificationQueueName"] ?? throw new ArgumentNullException("HospitalNotificationQueueName not found.");
            _hospitalNotificationRoutingKeyName = _configuration["RabbitMQ:HospitalNotificationRoutingKey"] ?? throw new ArgumentNullException("HospitalNotificationRoutingKey not found.");
            _hospitalNotificationExchangeName = _configuration["RabbitMQ:HospitalNotificationExchangeName"] ?? throw new ArgumentNullException("HospitalNotificationExchangeName not found.");
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[StockWorker] - Iniciando...");

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[StockWorker] - Rodando e ouvindo RabbitMQ...");

            // Consumir mensagens da fila de doações
            _ = Task.Run(async () =>
            {
                await _messageBus.ConsumeDirectMessagesAsync<DonationRegisteredEventDTO>(
                    _stockEventsExchangeName,
                    _donationQueueName, 
                    _donationRoutingKey,
                    async (donationEventDTO) =>
                    {
                        await ProcessDonation(donationEventDTO, stoppingToken);
                    }, stoppingToken);
            }, stoppingToken);

            // Consumir mensagens da fila de transfusões
            _ = Task.Run(async () =>
            {
                await _messageBus.ConsumeDirectMessagesAsync<BloodTransfusionRequestedEventDTO>(
                    _stockEventsExchangeName,
                    _transfusionQueueName,
                    _transfusionRoutingKey,
                    async (transfusionEventDTO) =>
                    {
                        await ProcessTransfusion(transfusionEventDTO, stoppingToken);
                    }, stoppingToken);
            }, stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task ProcessTransfusion(BloodTransfusionRequestedEventDTO transfusionEventDTO, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"[StockWorker] - Processando transfusão para hospital {transfusionEventDTO.HospitalId}: {transfusionEventDTO.QuantityML}ml de {transfusionEventDTO.BloodType} {transfusionEventDTO.RHFactor}");

            using var scope = _scopeFactory.CreateScope();

            var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            var stockRepository = scope.ServiceProvider.GetRequiredService<IBloodStockRepository>();
            var bloodTransfusionRepository = scope.ServiceProvider.GetRequiredService<IBloodTransfusionRepository>();
            var adminRepository = scope.ServiceProvider.GetRequiredService<IAdminRepository>();

            var stock = await stockRepository.GetBloodByRHAndTypeAsync(transfusionEventDTO.BloodType, transfusionEventDTO.RHFactor);

            if (stock == null || stock.QuantityML < transfusionEventDTO.QuantityML)
            {
                _logger.LogWarning($"[StockWorker] - Estoque insuficiente para transfusão: disponível {stock?.QuantityML ?? 0}ml, solicitado {transfusionEventDTO.QuantityML}ml");

                var admins = await adminRepository.GetAdministratorsAsync();
                var lowStockAlertDTO = new LowStockAlertEventDTO(transfusionEventDTO.BloodType, transfusionEventDTO.RHFactor, stock?.QuantityML ?? 0, admins);

                _logger.LogInformation($"[StockWorker] - Publicando alerta de estoque baixo para administradores.");
                await messageBus.PublishDirectMessageAsync(_lowStockExchangeName, _lowStockQueueName, _lowStockRoutingKeyName, lowStockAlertDTO);

                // Notificar o hospital que a transfusão foi negada
                var notification = new BloodTransfusionNotificationEventDTO(
                    transfusionEventDTO.HospitalId,
                    transfusionEventDTO.HospitalName,
                    transfusionEventDTO.HospitalEmail,
                    transfusionEventDTO.BloodType,
                    transfusionEventDTO.RHFactor,
                    transfusionEventDTO.QuantityML,
                    "Recusada"
                );

                _logger.LogInformation($"[StockWorker] - Notificando hospital {transfusionEventDTO.HospitalId} sobre falta de estoque.");
                await messageBus.PublishDirectMessageAsync(_hospitalNotificationExchangeName, _hospitalNotificationQueueName, _hospitalNotificationRoutingKeyName, notification);

                return;
            }

            // Atualizar estoque
            stock.QuantityML -= transfusionEventDTO.QuantityML;
            await stockRepository.UpdateQuantityFromStockAsync(stock);

            _logger.LogInformation($"[StockWorker] - Estoque atualizado após transfusão: {stock.QuantityML}ml restantes.");

            // Notificar o hospital que a transfusão foi realizada com sucesso
            var successNotification = new BloodTransfusionNotificationEventDTO(
                transfusionEventDTO.HospitalId,
                transfusionEventDTO.HospitalName,
                transfusionEventDTO.HospitalEmail,
                transfusionEventDTO.BloodType,
                transfusionEventDTO.RHFactor,
                transfusionEventDTO.QuantityML,
                "Confirmada"
            );

            _logger.LogInformation($"[StockWorker] - Persistindo transfusão de sangue.");

            var bloodTransfusion = new BloodTransfusionVO(
                transfusionEventDTO.HospitalId,
                transfusionEventDTO.QuantityML,
                transfusionEventDTO.BloodType,
                transfusionEventDTO.RHFactor
            );

            await bloodTransfusionRepository.AddBloodTransfusionAsync(bloodTransfusion);

            _logger.LogInformation($"[StockWorker] - Notificando hospital {transfusionEventDTO.HospitalId} sobre a conclusão da transfusão.");
            await messageBus.PublishDirectMessageAsync(_hospitalNotificationExchangeName, _hospitalNotificationQueueName, _hospitalNotificationRoutingKeyName, successNotification);
        }

        private async Task ProcessDonation(DonationRegisteredEventDTO donationEventDTO, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[StockWorker] - Processando doação: Doador: {donationEventDTO.DonorId} - {donationEventDTO.Quantity}ml de {donationEventDTO.BloodType} - {donationEventDTO.RHFactor}");

            using var scope = _scopeFactory.CreateScope();

            var adminRepository = scope.ServiceProvider.GetRequiredService<IAdminRepository>();
            var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            var stockRepository = scope.ServiceProvider.GetRequiredService<IBloodStockRepository>();

            var stock = await stockRepository.GetBloodByRHAndTypeAsync(donationEventDTO.BloodType, donationEventDTO.RHFactor);

            if (stock == null)
            {
                stock = new BloodStock
                {
                    BloodType = donationEventDTO.BloodType,
                    QuantityML = donationEventDTO.Quantity
                };
                await stockRepository.AddBloodToStockAsync(stock);
            }
            else
            {
                stock.QuantityML += donationEventDTO.Quantity;
                await stockRepository.UpdateQuantityFromStockAsync(stock);
            }

            _logger.LogInformation($"[StockWorker] - Publicando mensagem de notificação para o doador {donationEventDTO.DonorId} com o comprovante de doação!");
            await messageBus.PublishDirectMessageAsync(_donorNotificationExchangeName, _donorNotificationQueueName, _donorNotificationRoutingKeyName, donationEventDTO);

            _logger.LogInformation("[StockWorker] Estoque atualizado com sucesso!");
        }
    }

}

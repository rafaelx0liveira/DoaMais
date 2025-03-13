using DoaMais.StockService.Model;
using DoaMais.StockService.Repository.Interface;
using DoaMais.MessageBus.Interface;
using DoaMais.StockService.DTOs;
using DoaMais.StockService.ValueObject;
using VaultService.Interface;

namespace DoaMais.StockService
{
    public class StockWorker : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly IVaultClient _vaultClient;
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
            IConfiguration configuration,
            IVaultClient vaultClient)
        {
            _messageBus = messageBus;
            _vaultClient = vaultClient;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;

            var stockEventsExchangeName = _configuration["KeyVaultSecrets:RabbitMQ:StockEventsExchange"] ?? throw new ArgumentNullException("StockEventsExchangeName not found.");
            _stockEventsExchangeName = _vaultClient.GetSecret(stockEventsExchangeName);

            var donationQueueName = _configuration["KeyVaultSecrets:RabbitMQ:DonationQueue"] ?? throw new ArgumentNullException("DonationQueueName not found.");
            var donationRoutingKey = _configuration["KeyVaultSecrets:RabbitMQ:DonationRoutingKey"] ?? throw new ArgumentNullException("DonationRoutingKey not found.");
            _donationQueueName = _vaultClient.GetSecret(donationQueueName);
            _donationRoutingKey = _vaultClient.GetSecret(donationRoutingKey);

            var transfusionQueueName = _configuration["KeyVaultSecrets:RabbitMQ:TransfusionQueue"] ?? throw new ArgumentNullException("TransfusionQueueName not found.");
            var transfusionRoutingKey = _configuration["KeyVaultSecrets:RabbitMQ:TransfusionRoutingKey"] ?? throw new ArgumentNullException("TransfusionRoutingKey not found.");
            _transfusionQueueName = _vaultClient.GetSecret(transfusionQueueName);
            _transfusionRoutingKey = _vaultClient.GetSecret(transfusionRoutingKey);

            var lowStockQueueName = _configuration["KeyVaultSecrets:RabbitMQ:LowStockAlertQueue"] ?? throw new ArgumentNullException("LowStockAlertQueueName not found.");
            var lowStockRoutingKey = _configuration["KeyVaultSecrets:RabbitMQ:LowStockRoutingKey"] ?? throw new ArgumentNullException("LowStockRoutingKeyName not found.");
            _lowStockQueueName = _vaultClient.GetSecret(lowStockQueueName);
            _lowStockRoutingKeyName = _vaultClient.GetSecret(lowStockRoutingKey);

            var lowStockExchangeName = _configuration["KeyVaultSecrets:RabbitMQ:LowStockAlertExchange"] ?? throw new ArgumentNullException("LowStockAlertExchangeName not found.");
            _lowStockExchangeName = _vaultClient.GetSecret(lowStockExchangeName);

            var donorNotificationQueueName = _configuration["KeyVaultSecrets:RabbitMQ:DonorNotificationQueue"] ?? throw new ArgumentNullException("DonorNotificationQueueName not found.");
            var donorNotificatioRoutingKey = _configuration["KeyVaultSecrets:RabbitMQ:DonorNotificationRoutingKey"] ?? throw new ArgumentNullException("DonorNotificationRoutingKey not found.");
            var donorNotificationExchangeName = _configuration["KeyVaultSecrets:RabbitMQ:DonorNotificationExchange"] ?? throw new ArgumentNullException("DonorNotificationExchangeName not found.");
            _donorNotificationQueueName = _vaultClient.GetSecret(donorNotificationQueueName);
            _donorNotificationRoutingKeyName = _vaultClient.GetSecret(donorNotificatioRoutingKey);
            _donorNotificationExchangeName = _vaultClient.GetSecret(donorNotificationExchangeName);

            var hospitalNotificationQueueName = _configuration["KeyVaultSecrets:RabbitMQ:HospitalNotificationQueue"] ?? throw new ArgumentNullException("HospitalNotificationQueueName not found.");
            var hospitalNotificationRoutingKey = _configuration["KeyVaultSecrets:RabbitMQ:HospitalNotificationRoutingKey"] ?? throw new ArgumentNullException("HospitalNotificationRoutingKey not found.");
            var hospitalNotificationExchangeName = _configuration["KeyVaultSecrets:RabbitMQ:HospitalNotificationExchange"] ?? throw new ArgumentNullException("HospitalNotificationExchangeName not found.");
            _hospitalNotificationQueueName = _vaultClient.GetSecret(hospitalNotificationQueueName);
            _hospitalNotificationRoutingKeyName = _vaultClient.GetSecret(hospitalNotificationRoutingKey);
            _hospitalNotificationExchangeName = _vaultClient.GetSecret(hospitalNotificationExchangeName);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[StockWorker] - Iniciando...");

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[StockWorker] - Rodando e ouvindo RabbitMQ...");

            // Consumir mensagens da fila de doa��es
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

            // Consumir mensagens da fila de transfus�es
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
            _logger.LogInformation($"[StockWorker] - Processando transfus�o para hospital {transfusionEventDTO.HospitalId}: {transfusionEventDTO.QuantityML}ml de {transfusionEventDTO.BloodType} {transfusionEventDTO.RHFactor}");

            using var scope = _scopeFactory.CreateScope();

            var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            var stockRepository = scope.ServiceProvider.GetRequiredService<IBloodStockRepository>();
            var bloodTransfusionRepository = scope.ServiceProvider.GetRequiredService<IBloodTransfusionRepository>();
            var adminRepository = scope.ServiceProvider.GetRequiredService<IAdminRepository>();

            var stock = await stockRepository.GetBloodByRHAndTypeAsync(transfusionEventDTO.BloodType, transfusionEventDTO.RHFactor);

            if (stock == null || stock.QuantityML < transfusionEventDTO.QuantityML)
            {
                _logger.LogWarning($"[StockWorker] - Estoque insuficiente para transfus�o: dispon�vel {stock?.QuantityML ?? 0}ml, solicitado {transfusionEventDTO.QuantityML}ml");

                var admins = await adminRepository.GetAdministratorsAsync();
                var lowStockAlertDTO = new LowStockAlertEventDTO(transfusionEventDTO.BloodType, transfusionEventDTO.RHFactor, stock?.QuantityML ?? 0, admins);

                _logger.LogInformation($"[StockWorker] - Publicando alerta de estoque baixo para administradores.");
                await messageBus.PublishDirectMessageAsync(_lowStockExchangeName, _lowStockQueueName, _lowStockRoutingKeyName, lowStockAlertDTO);

                // Notificar o hospital que a transfus�o foi negada
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

            _logger.LogInformation($"[StockWorker] - Estoque atualizado ap�s transfus�o: {stock.QuantityML}ml restantes.");

            // Notificar o hospital que a transfus�o foi realizada com sucesso
            var successNotification = new BloodTransfusionNotificationEventDTO(
                transfusionEventDTO.HospitalId,
                transfusionEventDTO.HospitalName,
                transfusionEventDTO.HospitalEmail,
                transfusionEventDTO.BloodType,
                transfusionEventDTO.RHFactor,
                transfusionEventDTO.QuantityML,
                "Confirmada"
            );

            _logger.LogInformation($"[StockWorker] - Persistindo transfus�o de sangue.");

            var bloodTransfusion = new BloodTransfusionVO(
                transfusionEventDTO.HospitalId,
                transfusionEventDTO.QuantityML,
                transfusionEventDTO.BloodType,
                transfusionEventDTO.RHFactor
            );

            await bloodTransfusionRepository.AddBloodTransfusionAsync(bloodTransfusion);

            _logger.LogInformation($"[StockWorker] - Notificando hospital {transfusionEventDTO.HospitalId} sobre a conclus�o da transfus�o.");
            await messageBus.PublishDirectMessageAsync(_hospitalNotificationExchangeName, _hospitalNotificationQueueName, _hospitalNotificationRoutingKeyName, successNotification);
        }

        private async Task ProcessDonation(DonationRegisteredEventDTO donationEventDTO, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[StockWorker] - Processando doa��o: Doador: {donationEventDTO.DonorId} - {donationEventDTO.Quantity}ml de {donationEventDTO.BloodType} - {donationEventDTO.RHFactor}");

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

            _logger.LogInformation($"[StockWorker] - Publicando mensagem de notifica��o para o doador {donationEventDTO.DonorId} com o comprovante de doa��o!");
            await messageBus.PublishDirectMessageAsync(_donorNotificationExchangeName, _donorNotificationQueueName, _donorNotificationRoutingKeyName, donationEventDTO);

            _logger.LogInformation("[StockWorker] Estoque atualizado com sucesso!");
        }
    }

}

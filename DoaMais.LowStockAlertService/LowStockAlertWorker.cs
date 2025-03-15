using DoaMais.LowStockAlertService.Services.Interface;
using DoaMais.LowStockAlertService.ValueObject;
using DoaMais.MessageBus.Interface;
using VaultService.Interface;
using ILogger = Serilog.ILogger;

namespace DoaMais.LowStockAlertService
{
    public class LowStockAlertWorker : BackgroundService
    {
        private readonly IVaultClient _vaultClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ISendEmailService _sendEmailService;
        private readonly IMessageBus _messageBus;

        private readonly string _projectPath;
        private readonly string _normalizedPath;

        private readonly string _lowStockQueueName;
        private readonly string _lowStockRoutingKeyName;
        private readonly string _lowStockExchangeName;

        public LowStockAlertWorker(
            IVaultClient vaultClient,
            ILogger logger, 
            ISendEmailService sendEmailService,
            IConfiguration configuration, 
            IMessageBus messageBus)
        {
            _vaultClient = vaultClient;
            _logger = logger;
            _sendEmailService = sendEmailService;
            _configuration = configuration;
            _messageBus = messageBus;

            _projectPath = Path.Combine(AppContext.BaseDirectory, "Templates/low_stock_template.html");
            _normalizedPath = Path.GetFullPath(_projectPath);

            var lowStockQueueName = _configuration["KeyVaultSecrets:RabbitMQ:LowStockAlertQueue"] ?? throw new ArgumentNullException("LowStockAlertQueueName not found.");
            _lowStockQueueName = _vaultClient.GetSecret(lowStockQueueName);

            var lowStockRoutingKeyName = _configuration["KeyVaultSecrets:RabbitMQ:LowStockRoutingKey"] ?? throw new ArgumentNullException("LowStockRoutingKeyName not found.");
            _lowStockRoutingKeyName = _vaultClient.GetSecret(lowStockRoutingKeyName);

            var lowStockExchangeName = _configuration["KeyVaultSecrets:RabbitMQ:LowStockAlertExchange"] ?? throw new ArgumentNullException("LowStockAlertExchangeName not found.");
            _lowStockExchangeName = _vaultClient.GetSecret(lowStockExchangeName);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("[LowStockAlertWorker] - Initiating RabbitMQ connection...");

            await _messageBus.ConsumeDirectMessagesAsync<LowStockAlertEventVO>(
                _lowStockExchangeName,
                _lowStockQueueName,
                _lowStockRoutingKeyName,
                async (lowStockEvent) =>
                {
                    _logger.Information($"[LowStockAlertWorker] - Critic stock alert for {lowStockEvent.BloodType} {lowStockEvent.RHFactor}: {lowStockEvent.QuantityML} ml.");

                    foreach (var adminEmail in lowStockEvent.AdminEmails)
                    {
                        Dictionary<string, string> placeholders = new Dictionary<string, string>
                        {
                            {"Administrator", adminEmail.Name },
                            {"BloodType", lowStockEvent.BloodType.ToString() },
                            {"RHFactor",  lowStockEvent.RHFactor.ToString() },
                            {"Quantity", lowStockEvent.QuantityML.ToString() }
                        };

                        try
                        {
                            await _sendEmailService.SendEmailAsync(
                                adminEmail.Email,
                                "ALERTA | ESTOQUE CRÍTICO DE SANGUE",
                                _normalizedPath,
                                placeholders
                            );

                            _logger.Information($"[LowStockAlertWorker] - Send email to administrator {adminEmail.ToString()}");
                        }
                        catch (Exception ex)
                        {
                            _logger.Warning($"[LowStockAlertWorker] - Error to send email to administrator {adminEmail.ToString()}. Error: {ex.Message}");
                        }
                    }
                },
                stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

    }
}

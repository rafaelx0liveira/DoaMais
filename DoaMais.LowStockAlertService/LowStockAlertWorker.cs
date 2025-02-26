using DoaMais.LowStockAlertService.Services.Interface;
using DoaMais.LowStockAlertService.ValueObject;
using DoaMais.MessageBus.Interface;

namespace DoaMais.LowStockAlertService
{
    public class LowStockAlertWorker : BackgroundService
    {
        private readonly ILogger<LowStockAlertWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISendEmailService _sendEmailService;
        private readonly IMessageBus _messageBus;

        private readonly string _projectPath;
        private readonly string _normalizedPath;

        private readonly string _lowStockQueueName;
        private readonly string _lowStockRoutingKeyName;
        private readonly string _lowStockExchangeName;

        public LowStockAlertWorker(ILogger<LowStockAlertWorker> logger, 
            ISendEmailService sendEmailService,
            IConfiguration configuration, 
            IMessageBus messageBus)
        {
            _logger = logger;
            _sendEmailService = sendEmailService;
            _configuration = configuration;
            _messageBus = messageBus;

            _projectPath = Path.Combine(AppContext.BaseDirectory, "Templates/low_stock_template.html");
            _normalizedPath = Path.GetFullPath(_projectPath);

            _lowStockQueueName = _configuration["RabbitMQ:LowStockAlertQueueName"] ?? throw new ArgumentNullException("LowStockAlertQueueName not found.");
            _lowStockRoutingKeyName = _configuration["RabbitMQ:LowStockRoutingKeyName"] ?? throw new ArgumentNullException("LowStockRoutingKeyName not found.");
            _lowStockExchangeName = _configuration["RabbitMQ:LowStockAlertExchangeName"] ?? throw new ArgumentNullException("LowStockAlertExchangeName not found.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("LowStockAlertWorker rodando e ouvindo RabbitMQ...");

            await _messageBus.ConsumeDirectMessagesAsync<LowStockAlertEventVO>(
                _lowStockExchangeName,
                _lowStockQueueName,
                _lowStockRoutingKeyName,
                async (lowStockEvent) =>
                {
                    _logger.LogInformation($"Estoque crítico para {lowStockEvent.BloodType} {lowStockEvent.RHFactor}: {lowStockEvent.QuantityML} ml.");

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

                            _logger.LogInformation($"Enviado alerta para administrador {adminEmail}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Erro ao enviar alerta para administrador {adminEmail}: {ex.Message}");
                        }
                    }
                },
                stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

    }
}

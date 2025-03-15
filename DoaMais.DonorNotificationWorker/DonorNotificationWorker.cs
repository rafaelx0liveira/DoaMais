using DoaMais.DonorNotificationService.Services.Interface;
using DoaMais.DonorNotificationWorker.ValueObject;
using DoaMais.MessageBus.Interface;
using VaultService.Interface;
using ILogger = Serilog.ILogger;

namespace DoaMais.DonorNotificationService
{
    public class DonorNotificationWorker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IVaultClient _vaultClient;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly ISendEmailService _sendEmailService;

        private readonly string _projectPath;
        private readonly string _normalizedPath;

        private readonly string _donorNotificationRoutingKeyName;
        private readonly string _donorNotificationExchangeName;
        private readonly string _donorNotificationQueueName;

        public DonorNotificationWorker(ILogger logger, IMessageBus messageBus , IConfiguration configuration, ISendEmailService sendEmailService, IVaultClient vaultClient)
        {
            _logger = logger;
            _messageBus = messageBus;
            _configuration = configuration;
            _sendEmailService = sendEmailService;
            _vaultClient = vaultClient;

            _projectPath = Path.Combine(AppContext.BaseDirectory, "Templates/email_template.html");
            _normalizedPath = Path.GetFullPath(_projectPath);

            var donorNotificationExchangeName = _configuration["KeyVaultSecrets:RabbitMQ:DonorNotificationExchange"] ?? throw new ArgumentNullException("DonorNotificationExchangeName not found.");
            _donorNotificationExchangeName = _vaultClient.GetSecret(donorNotificationExchangeName);

            var donorNotificationQueueName = _configuration["KeyVaultSecrets:RabbitMQ:DonorNotificationQueue"] ?? throw new ArgumentNullException("DonorNotificationQueueName not found.");
            _donorNotificationQueueName = _vaultClient.GetSecret(donorNotificationQueueName);

            var donorNotificationRoutingKey = _configuration["KeyVaultSecrets:RabbitMQ:DonorNotificationRoutingKey"] ?? throw new ArgumentNullException("DonorNotificationRoutingKey not found.");
            _donorNotificationRoutingKeyName = _vaultClient.GetSecret(donorNotificationRoutingKey);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("[DonorNotificationWorker] - Initiating RabbitMQ connection...");

            await _messageBus.ConsumeDirectMessagesAsync<DonationRegisteredEventVO>(
                _donorNotificationExchangeName, 
                _donorNotificationQueueName, 
                _donorNotificationRoutingKeyName,
                async (donationEvent) =>
                {
                    _logger.Information($"[DonorNotificationWorker] - Sending email to donor {donationEvent.DonorEmail} - Id: {donationEvent.DonorId}");

                    Dictionary<string, string> placeholders = new Dictionary<string, string>
                    {
                        {"DonorName", donationEvent.DonorName },
                        {"DonorEmail", donationEvent.DonorEmail },
                        {"BloodType", donationEvent.BloodType.ToString() },
                        {"RHFactor",  donationEvent.RHFactor.ToString() },
                        {"Quantity", donationEvent.Quantity.ToString() }
                    };

                    try
                    {
                        await _sendEmailService.SendEmailAsync(
                            donationEvent.DonorEmail,
                            "COMPROVANTE | DOAÇÃO DE SANGUE",
                            _normalizedPath,
                            placeholders
                        );

                        _logger.Information($"[DonorNotificationWorker] - Successfull email sent to donor {donationEvent.DonorEmail} - Id: {donationEvent.DonorId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning($"[DonorNotificationWorker] - Error to send email to donor {donationEvent.DonorEmail} - Id: {donationEvent.DonorId}. Error: {ex.Message}");
                    }
                }, stoppingToken);
        }
    }
}

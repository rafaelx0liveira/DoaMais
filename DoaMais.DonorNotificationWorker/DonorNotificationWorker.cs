using DoaMais.DonorNotificationService.Services.Interface;
using DoaMais.DonorNotificationWorker.ValueObject;
using DoaMais.MessageBus.Interface;

namespace DoaMais.DonorNotificationService
{
    public class DonorNotificationWorker : BackgroundService
    {
        private readonly ILogger<DonorNotificationWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly ISendEmailService _sendEmailService;

        private readonly string _projectPath;
        private readonly string _normalizedPath;

        private readonly string _donorNotificationRoutingKeyName;
        private readonly string _donorNotificationExchangeName;
        private readonly string _donorNotificationQueueName;

        public DonorNotificationWorker(ILogger<DonorNotificationWorker> logger, IMessageBus messageBus , IConfiguration configuration, ISendEmailService sendEmailService)
        {
            _logger = logger;
            _messageBus = messageBus;
            _configuration = configuration;
            _sendEmailService = sendEmailService;

            _projectPath = Path.Combine(AppContext.BaseDirectory, "Templates/email_template.html");
            _normalizedPath = Path.GetFullPath(_projectPath);

            _donorNotificationExchangeName = _configuration["RabbitMQ:DonorNotificationExchangeName"] ?? throw new ArgumentNullException("DonorNotificationExchangeName not found.");
            _donorNotificationQueueName = _configuration["RabbitMQ:DonorNotificationQueueName"] ?? throw new ArgumentNullException("DonorNotificationQueueName not found.");
            _donorNotificationRoutingKeyName = _configuration["RabbitMQ:DonorNotificationRoutingKey"] ?? throw new ArgumentNullException("DonorNotificationRoutingKey not found.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[DonorNotificationService] - Rodando e ouvindo RabbitMQ...");

            await _messageBus.ConsumeDirectMessagesAsync<DonationRegisteredEventVO>(
                _donorNotificationExchangeName, 
                _donorNotificationQueueName, 
                _donorNotificationRoutingKeyName,
                async (donationEvent) =>
                {
                    _logger.LogInformation($"[DonorNotificationService] - Enviando email para o doador {donationEvent.DonorEmail} - Id: {donationEvent.DonorId}");

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

                        _logger.LogInformation($"[DonorNotificationService] - Enviado email para o doador {donationEvent.DonorEmail} - Id: {donationEvent.DonorId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"[DonorNotificationService] - Erro ao enviar email para o doador {donationEvent.DonorEmail} - Id: {donationEvent.DonorId}");
                    }
                }, stoppingToken);
        }
    }
}

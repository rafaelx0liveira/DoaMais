using DoaMais.HospitalNotificationService.Services.Interface;
using DoaMais.HospitalNotificationService.ValueObjects;
using DoaMais.MessageBus.Interface;
using VaultService.Interface;
using ILogger = Serilog.ILogger;

namespace DoaMais.HospitalNotificationService
{
    public class HospitalWorker : BackgroundService
    {
        private readonly IVaultClient _vaultClient;
        private readonly ISendEmailService _sendEmailService;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;

        private readonly string _hospitalNotificationQueueName;
        private readonly string _hospitalNotificationRoutingKeyName;
        private readonly string _hospitalNotificationExchangeName;

        private readonly string _projectPath;
        private readonly string _normalizedPath;

        private readonly Dictionary<string, string> _emailSettings = new();

        public HospitalWorker(
            IVaultClient vaultClient,
            ILogger logger,
            IConfiguration configuration,
            IMessageBus messageBus,
            ISendEmailService sendEmailService)
        {
            _vaultClient = vaultClient;
            _logger = logger;
            _messageBus = messageBus;
            _configuration = configuration;
            _sendEmailService = sendEmailService;

            var hospitalNotificationQueueName = _configuration["KeyVaultSecrets:RabbitMQ:HospitalNotificationQueue"] ?? throw new ArgumentNullException("HospitalNotificationQueueName not found.");
            _hospitalNotificationQueueName = _vaultClient.GetSecret(hospitalNotificationQueueName);

            var hospitalNotificationRoutingKey = _configuration["KeyVaultSecrets:RabbitMQ:HospitalNotificationRoutingKey"] ?? throw new ArgumentNullException("HospitalNotificationRoutingKey not found.");
            _hospitalNotificationRoutingKeyName = _vaultClient.GetSecret(hospitalNotificationRoutingKey);

            var hospitalNotificationExchangeName = _configuration["KeyVaultSecrets:RabbitMQ:HospitalNotificationExchange"] ?? throw new ArgumentNullException("HospitalNotificationExchangeName not found.");
            _hospitalNotificationExchangeName = _vaultClient.GetSecret(hospitalNotificationExchangeName);

            _projectPath = Path.Combine(AppContext.BaseDirectory, "Templates/hospital_notification_template.html");
            _normalizedPath = Path.GetFullPath(_projectPath);

            // Configurações de email para cada status
            _emailSettings["Confirmada"] = "✅ SOLICITAÇÃO CONFIRMADA|#2e7d32|#d4edda|#155724|#c3e6cb|A solicitação de sangue para transfusão foi realizada com sucesso.|O sangue solicitado foi enviado com sucesso ao hospital.";
            _emailSettings["Recusada"] = "⚠️ SOLICITAÇÃO NÃO REALIZADA|#d32f2f|#f8d7da|#721c24|#f5c6cb|Não há sangue suficiente no estoque.|Pedimos desculpas pelo transtorno. Reabastecemos o estoque e entraremos em contato assim que possível.";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("[HospitalWorker] - Initiating RabbitMQ connection...");

            await _messageBus.ConsumeDirectMessagesAsync<BloodTransfusionNotificationEventVO>(
                _hospitalNotificationExchangeName,
                _hospitalNotificationQueueName,
                _hospitalNotificationRoutingKeyName,
                async (bloodTransfusionNotificationVO) =>
                {
                    _logger.Information($"[HospitalWorker] - Notifying hospital {bloodTransfusionNotificationVO.HospitalId} about blood transfusion request status.");

                    // Obter configurações de email conforme o status da solicitação
                    string statusKey = bloodTransfusionNotificationVO.Status.Equals("Confirmada") ? "Confirmada" : "Recusada";
                    var emailConfig = _emailSettings[statusKey].Split('|');

                    Dictionary<string, string> placeholders = new()
                    {
                        { "HeaderTitle", emailConfig[0] },
                        { "HeaderColor", emailConfig[1] },
                        { "StatusBgColor", emailConfig[2] },
                        { "StatusTextColor", emailConfig[3] },
                        { "StatusBorderColor", emailConfig[4] },
                        { "StatusMessage", emailConfig[5] },
                        { "AdditionalMessage", emailConfig[6] },
                        { "HospitalName", bloodTransfusionNotificationVO.HospitalName },
                        { "BloodType", bloodTransfusionNotificationVO.BloodType.ToString() },
                        { "RHFactor", bloodTransfusionNotificationVO.RHFactor.ToString() },
                        { "Quantity", bloodTransfusionNotificationVO.QuantityML.ToString() },
                        { "Status", bloodTransfusionNotificationVO.Status }
                    };

                    try
                    {
                        await _sendEmailService.SendEmailAsync(
                            bloodTransfusionNotificationVO.HospitalEmail,
                            "Notificação da Solicitação de Sangue | DoaMais",
                            _normalizedPath,
                            placeholders
                        );

                        _logger.Information($"[HospitalWorker] - Hospital {bloodTransfusionNotificationVO.HospitalId} successfully notified!");
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning($"[HospitalWorker] - Error to send email to hospital {bloodTransfusionNotificationVO.HospitalId}. Error: {ex.Message}");

                        // Republicando a mensagem na fila para tentar enviar novamente
                        await _messageBus.PublishDirectMessageAsync(
                            _hospitalNotificationExchangeName,
                            _hospitalNotificationQueueName,
                            _hospitalNotificationRoutingKeyName,
                            bloodTransfusionNotificationVO
                        );

                        _logger.Information($"[HospitalWorker] - Message republished to queue for new attempt.");
                    }

                }, stoppingToken);
        }
    }
}

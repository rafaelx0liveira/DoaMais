using DoaMais.HospitalNotificationService.Services.Interface;
using DoaMais.HospitalNotificationService.ValueObjects;
using DoaMais.MessageBus.Interface;

namespace DoaMais.HospitalNotificationService
{
    public class HospitalWorker : BackgroundService
    {
        private readonly ISendEmailService _sendEmailService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<HospitalWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;

        private readonly string _hospitalNotificationQueueName;
        private readonly string _hospitalNotificationRoutingKeyName;
        private readonly string _hospitalNotificationExchangeName;

        private readonly string _projectPath;
        private readonly string _normalizedPath;

        private readonly Dictionary<string, string> _emailSettings = new();

        public HospitalWorker(
            ILogger<HospitalWorker> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            IMessageBus messageBus,
            ISendEmailService sendEmailService)
        {
            _logger = logger;
            _messageBus = messageBus;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _sendEmailService = sendEmailService;

            _hospitalNotificationQueueName = _configuration["RabbitMQ:HospitalNotificationQueueName"] ?? throw new ArgumentNullException("HospitalNotificationQueueName not found.");
            _hospitalNotificationRoutingKeyName = _configuration["RabbitMQ:HospitalNotificationRoutingKey"] ?? throw new ArgumentNullException("HospitalNotificationRoutingKey not found.");
            _hospitalNotificationExchangeName = _configuration["RabbitMQ:HospitalNotificationExchangeName"] ?? throw new ArgumentNullException("HospitalNotificationExchangeName not found.");

            _projectPath = Path.Combine(AppContext.BaseDirectory, "Templates/hospital_notification_template.html");
            _normalizedPath = Path.GetFullPath(_projectPath);

            // Configurações de email para cada status
            _emailSettings["Confirmada"] = "✅ SOLICITAÇÃO CONFIRMADA|#2e7d32|#d4edda|#155724|#c3e6cb|A solicitação de sangue para transfusão foi realizada com sucesso.|O sangue solicitado foi enviado com sucesso ao hospital.";
            _emailSettings["Recusada"] = "⚠️ SOLICITAÇÃO NÃO REALIZADA|#d32f2f|#f8d7da|#721c24|#f5c6cb|Não há sangue suficiente no estoque.|Pedimos desculpas pelo transtorno. Reabastecemos o estoque e entraremos em contato assim que possível.";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[HospitalWorker] - Rodando e ouvindo RabbitMQ...");

            await _messageBus.ConsumeDirectMessagesAsync<BloodTransfusionNotificationEventVO>(
                _hospitalNotificationExchangeName,
                _hospitalNotificationQueueName,
                _hospitalNotificationRoutingKeyName,
                async (bloodTransfusionNotificationVO) =>
                {
                    _logger.LogInformation($"[HospitalWorker] - Notificando hospital {bloodTransfusionNotificationVO.HospitalId} sobre status da solicitação de transfusão.");

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

                        _logger.LogInformation($"[HospitalWorker] - Hospital {bloodTransfusionNotificationVO.HospitalId} notificado com sucesso!");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"[HospitalWorker] - Erro ao notificar hospital {bloodTransfusionNotificationVO.HospitalId}: {ex.Message}");

                        // Republicando a mensagem na fila para tentar enviar novamente
                        await _messageBus.PublishDirectMessageAsync(
                            _hospitalNotificationExchangeName,
                            _hospitalNotificationQueueName,
                            _hospitalNotificationRoutingKeyName,
                            bloodTransfusionNotificationVO
                        );

                        _logger.LogInformation($"[HospitalWorker] - Mensagem reenviada para a fila para nova tentativa.");
                    }

                }, stoppingToken);
        }
    }
}

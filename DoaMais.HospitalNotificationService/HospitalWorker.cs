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

        private readonly string _confirmedColor = "#2e7d32";
        private readonly string _refusedColor = "#d32f2f";

        private readonly string _confirmedHeader = "✅ SOLICITAÇÃO CONFIRMADA";
        private readonly string _refusedHeader = "⚠️ SOLITICAÇÃO NÃO REALIZADA";

        private readonly string _confirmedStatusBgColor = "#d4edda";
        private readonly string _refusedStatusBgColor = "#f8d7da";

        private readonly string _confirmedStatusTextColor = "#155724";
        private readonly string _refusedStatusTextColor = "#721c24";

        private readonly string _confirmedStatusBorderColor = "#c3e6cb";
        private readonly string _refusedStatusBorderColor = "#f5c6cb";

        private readonly string _confirmedStatusMessage = "A solicitação de sangue para transfusão foi realizada com sucesso.";
        private readonly string _refusedStatusMessage = "Não há sangue suficiente no estoque.";

        private readonly string _confirmedAdditionalMessage = "O sangue solicitado foi enviado com sucesso ao hospital.";
        private readonly string _refusedAdditionalMessage = "Pedimos desculpas pelo transtorno. Reabastecemos o estoque e entraremos em contato assim que possível.";

        public HospitalWorker(ILogger<HospitalWorker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration, IMessageBus messageBus, ISendEmailService sendEmailService)
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

                    Dictionary<string, string> placeholders = new Dictionary<string, string>();

                    if (bloodTransfusionNotificationVO.Status.Equals("Confirmada"))
                    {
                        placeholders.Add("HeaderColor", _confirmedColor);
                        placeholders.Add("HeaderTitle", _confirmedHeader);
                        placeholders.Add("StatusBgColor", _confirmedStatusBgColor);
                        placeholders.Add("StatusTextColor", _confirmedStatusTextColor);
                        placeholders.Add("StatusBorderColor", _confirmedStatusBorderColor);
                        placeholders.Add("StatusMessage", _confirmedStatusMessage);
                        placeholders.Add("AdditionalMessage", _confirmedAdditionalMessage);
                        placeholders.Add("HospitalName", bloodTransfusionNotificationVO.HospitalName);
                        placeholders.Add("BloodType", bloodTransfusionNotificationVO.BloodType.ToString());
                        placeholders.Add("RHFactor", bloodTransfusionNotificationVO.RHFactor.ToString());
                        placeholders.Add("Quantity", bloodTransfusionNotificationVO.QuantityML.ToString());
                        placeholders.Add("Status", bloodTransfusionNotificationVO.Status);
                    }
                    else
                    {
                        placeholders.Add("HeaderColor", _refusedColor);
                        placeholders.Add("HeaderTitle", _refusedHeader);
                        placeholders.Add("StatusBgColor", _refusedStatusBgColor);
                        placeholders.Add("StatusTextColor", _refusedStatusTextColor);
                        placeholders.Add("StatusBorderColor", _refusedStatusBorderColor);
                        placeholders.Add("StatusMessage", _refusedStatusMessage);
                        placeholders.Add("AdditionalMessage", _refusedAdditionalMessage);
                        placeholders.Add("HospitalName", bloodTransfusionNotificationVO.HospitalName);
                        placeholders.Add("BloodType", bloodTransfusionNotificationVO.BloodType.ToString());
                        placeholders.Add("RHFactor", bloodTransfusionNotificationVO.RHFactor.ToString());
                        placeholders.Add("Quantity", bloodTransfusionNotificationVO.QuantityML.ToString());
                        placeholders.Add("Status", bloodTransfusionNotificationVO.Status);
                    }

                    try
                    {
                        await _sendEmailService.SendEmailAsync(
                            bloodTransfusionNotificationVO.HospitalEmail,
                            "Notificação da Solicitação de Sangue | DoaMais",
                            _normalizedPath,
                            placeholders
                            );

                        _logger.LogInformation($"[HospitalWorker] - Hospital {bloodTransfusionNotificationVO.HospitalId} notificado sobre status da solicitação de transfusão!");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"[HospitalWorker] - Erro ao notificar hospital {bloodTransfusionNotificationVO.HospitalId}: {ex.Message}");
                    }

                }, stoppingToken);
        }
    }
}

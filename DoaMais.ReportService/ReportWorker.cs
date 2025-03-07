using DoaMais.ReportService.Services.Interface;

namespace DoaMais.ReportService
{
    public class ReportWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReportWorker> _logger;
        private readonly TimeSpan _intervaloExecucao = TimeSpan.FromDays(30); // A cada 30 dias

        public ReportWorker(IServiceProvider serviceProvider, ILogger<ReportWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var reportService = scope.ServiceProvider.GetRequiredService<IReportService>();
                        _logger.LogInformation("[ReportWorker] - Iniciando geração automática de relatórios...");

                        _logger.LogInformation("[ReportWorker] - Gerando relatório de estoque de sangue...");
                        await reportService.GenerateBloodStockReport();

                        _logger.LogInformation("[ReportWorker] - Gerando relatório de doações nos últimos 30 dias...");
                        await reportService.GenerateDonationsReport();

                        _logger.LogInformation("[ReportWorker] - Relatórios gerados com sucesso!");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[ReportWorker] - Erro ao gerar os relatórios: {ex.Message}");
                }

                // Espera 30 dias antes de rodar novamente
                await Task.Delay(_intervaloExecucao, stoppingToken);
            }
        }
    }

}

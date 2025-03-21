using DoaMais.ReportService.Services.Interface;
using ILogger = Serilog.ILogger;

namespace DoaMais.ReportService
{
    public class ReportWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly TimeSpan _executionInterval = TimeSpan.FromDays(30);

        public ReportWorker(IServiceProvider serviceProvider, ILogger logger)
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
                        _logger.Information($"[ReportWorker] - Initiating automatic report generation at {DateTime.UtcNow.AddHours(-3)}...");
                        var reportService = scope.ServiceProvider.GetRequiredService<IReportService>();

                        _logger.Information("[ReportWorker] - Generating blood stock report...");
                        await reportService.GenerateAndSaveBloodStockReport();

                        _logger.Information("[ReportWorker] - Generating donations report that occurred in the last 30 days...");
                        await reportService.GenerateAndSaveDonationsReport();

                        _logger.Information($"[ReportWorker] - Automatic report generation completed at {DateTime.UtcNow.AddHours(-3)}.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"[ReportWorker] - Error generating reports: {ex.Message}");
                }

                await Task.Delay(_executionInterval, stoppingToken);
            }
        }
    }

}

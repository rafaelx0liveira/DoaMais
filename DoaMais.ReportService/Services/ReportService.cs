
using DoaMais.ReportService.Repository.Interface;
using DoaMais.ReportService.Services.Interface;
using System.Text;

namespace DoaMais.ReportService.Services
{
    public class ReportService : IReportService
    {
        private readonly IBloodStockRepository _bloodStockRepository;
        private readonly IDonationRepository _donationRepository;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            IBloodStockRepository bloodStockRepository,
            IDonationRepository donationRepository,
            ILogger<ReportService> logger)
        {
            _bloodStockRepository = bloodStockRepository;
            _donationRepository = donationRepository;
            _logger = logger;
        }

        public async Task GenerateBloodStockReport()
        {
            var bloodStock = await _bloodStockRepository.GetAllAsync();
            var report = new StringBuilder();

            report.AppendLine("[ReportWorker] - Relatório de Estoque de Sangue");
            foreach (var stock in bloodStock)
            {
                report.AppendLine($"[ReportWorker] -  {stock.BloodType} {stock.RHFactor}: {stock.QuantityML} ml disponíveis | Data {DateTime.UtcNow}");
            }

            _logger.LogInformation(report.ToString());
            await SaveReport("BloodStockReport.txt", report.ToString());
        }

        public async Task GenerateDonationsReport()
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);
            var donations = await _donationRepository.GetDonationsInPeriodAsync(last30Days, DateTime.UtcNow);
            var report = new StringBuilder();

            report.AppendLine("[ReportWorker] - Relatório de Doações nos Últimos 30 Dias");
            foreach (var donation in donations)
            {
                report.AppendLine($"[ReportWorker] - ID: {donation.DonorId} | Data: {donation.DonationDate} | Quantidade: {donation.QuantityML} ml");
            }

            _logger.LogInformation(report.ToString());
            await SaveReport("DonationsReport.txt", report.ToString());
        }

        private async Task SaveReport(string fileName, string content)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", fileName);
            await File.WriteAllTextAsync(path, content);
        }
    }

}

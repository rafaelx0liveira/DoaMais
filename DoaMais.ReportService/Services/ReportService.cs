
using DoaMais.ReportService.Model;
using DoaMais.ReportService.Model.Enums;
using DoaMais.ReportService.Reports.BloodStockReports;
using DoaMais.ReportService.Reports.DonationsReports;
using DoaMais.ReportService.Repository.Interface;
using DoaMais.ReportService.Services.Interface;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Text;

namespace DoaMais.ReportService.Services
{
    public class ReportService : IReportService
    {
        private readonly IBloodStockRepository _bloodStockRepository;
        private readonly IDonationRepository _donationRepository;
        private readonly IReportRepository _reportRepository;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            IBloodStockRepository bloodStockRepository,
            IDonationRepository donationRepository,
            IReportRepository reportRepository,
            ILogger<ReportService> logger)
        {
            _bloodStockRepository = bloodStockRepository;
            _donationRepository = donationRepository;
            _reportRepository = reportRepository;
            _logger = logger;
        }

        public async Task GenerateAndSaveBloodStockReport()
        {
            var bloodStock = await _bloodStockRepository.GetAllAsync();

            var pdfBytes = new BloodStockReport(bloodStock).GeneratePdf();

            var report = new Report
            {
                Name = $"Relatório_Estoque_Sangue_{DateTime.UtcNow.AddHours(-3):yyyyMMddHHmm}.pdf",
                FileData = pdfBytes,
                ReportType = ReportType.BloodStock
            };

            await _reportRepository.AddAsync(report);
            _logger.LogInformation("[ReportWorker] - Relatório de estoque de sangue salvo no banco.");
        }

        public async Task GenerateAndSaveDonationsReport()
        {
            var last30Days = DateTime.UtcNow.AddHours(-3).AddDays(-30);
            var donations = await _donationRepository.GetDonationsInPeriodAsync(last30Days, DateTime.UtcNow.AddHours(-3));
            var pdfBytes = new DonationReport(donations).GeneratePdf();

            var report = new Report
            {
                Name = $"Relatório_Doações_{DateTime.UtcNow.AddHours(-3):yyyyMMddHHmm}.pdf",
                FileData = pdfBytes,
                ReportType = ReportType.Donations
            };

            await _reportRepository.AddAsync(report);
            _logger.LogInformation("[ReportWorker] - Relatório de doações salvo no banco.");
        }
    }
}

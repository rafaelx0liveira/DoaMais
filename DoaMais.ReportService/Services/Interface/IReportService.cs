
namespace DoaMais.ReportService.Services.Interface
{
    public interface IReportService
    {
        Task GenerateBloodStockReport();
        Task GenerateDonationsReport();
    }
}

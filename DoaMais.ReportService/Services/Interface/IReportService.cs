
namespace DoaMais.ReportService.Services.Interface
{
    public interface IReportService
    {
        Task GenerateAndSaveBloodStockReport();
        Task GenerateAndSaveDonationsReport();
    }
}

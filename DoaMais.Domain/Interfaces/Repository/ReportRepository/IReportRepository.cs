using DoaMais.Domain.Entities;

namespace DoaMais.Domain.Interfaces.Repository.ReportRepository
{
    public interface IReportRepository
    {
        Task<Report> GetLastBloodStockReportAsync();
        Task<Report> GetLastDonationsReportAsync();
    }
}

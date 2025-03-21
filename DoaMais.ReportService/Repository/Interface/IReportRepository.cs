
using DoaMais.ReportService.Model;

namespace DoaMais.ReportService.Repository.Interface
{
    public interface IReportRepository 
    {
        Task AddAsync(Report report);
    }
}

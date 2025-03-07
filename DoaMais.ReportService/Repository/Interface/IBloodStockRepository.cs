using DoaMais.ReportService.Model;

namespace DoaMais.ReportService.Repository.Interface
{
    public interface IBloodStockRepository
    {
        Task<IEnumerable<BloodStock>> GetAllAsync();
    }
}

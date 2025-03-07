using DoaMais.ReportService.Model;

namespace DoaMais.ReportService.Repository.Interface
{
    public interface IDonationRepository
    {
        Task<List<Donation>> GetDonationsInPeriodAsync(DateTime startDate, DateTime endDate);
    }

}

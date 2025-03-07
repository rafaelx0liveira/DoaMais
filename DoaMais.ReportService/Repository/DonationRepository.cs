using DoaMais.ReportService.Model;
using DoaMais.ReportService.Model.Context;
using DoaMais.ReportService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.ReportService.Repository
{
    public class DonationRepository (SQLServerContext context) : IDonationRepository
    {
        private readonly SQLServerContext _context = context;

        public async Task<List<Donation>> GetDonationsInPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Donations
                .Where(d => d.DonationDate >= startDate && d.DonationDate <= endDate)
                .ToListAsync();
        }
    }
}

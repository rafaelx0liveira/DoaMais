using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;
using DoaMais.Domain.Interfaces.Repository.ReportRepository;
using DoaMais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.Infrastructure.Repositories.ReportRepository
{
    public class ReportRepository(SQLServerContext context)
        : IReportRepository
    {
        private readonly SQLServerContext _context = context;

        public async Task<Report> GetLastBloodStockReportAsync()
        {
            return await _context.Reports
                .Where(r => r.ReportType == ReportType.BloodStock)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Report> GetLastDonationsReportAsync()
        {
            return await _context.Reports
                .Where(r => r.ReportType == ReportType.Donations)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}

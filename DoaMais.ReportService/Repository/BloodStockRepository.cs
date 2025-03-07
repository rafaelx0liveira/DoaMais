using DoaMais.ReportService.Model;
using DoaMais.ReportService.Model.Context;
using DoaMais.ReportService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.ReportService.Repository
{
    public class BloodStockRepository(SQLServerContext context) : IBloodStockRepository
    {
        private readonly SQLServerContext _context = context;

        public async Task<IEnumerable<BloodStock>> GetAllAsync()
        {
            return await _context.BloodStocks
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

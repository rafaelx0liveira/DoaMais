using DoaMais.StockService.Model;
using DoaMais.StockService.Model.Context;
using DoaMais.StockService.Model.Enums;
using DoaMais.StockService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.StockService.Repository
{
    public class BloodStockRepository(SQLServerContext context) : IBloodStockRepository
    {
        private readonly SQLServerContext _context = context;

        public async Task AddBloodToStockAsync(BloodStock blood)
        {
            await _context.BloodStocks.AddAsync(blood);
            await _context.SaveChangesAsync();
        }

        public Task<BloodStock> GetBloodByRHAndTypeAsync(BloodType type, RHFactor rhFactor)
        {
            return _context
                    .BloodStocks
                    .FirstOrDefaultAsync(x => x.BloodType == type && x.RHFactor == rhFactor);
        }

        public async Task UpdateQuantityFromStockAsync(BloodStock bloodStock)
        {
            _context.BloodStocks.Update(bloodStock);
            await _context.SaveChangesAsync();
        }
    }
}

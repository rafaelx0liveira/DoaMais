using DoaMais.Domain.Entities;
using DoaMais.Domain.Interfaces.Repository.BloodTransfusionRepository;
using DoaMais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.Infrastructure.Repositories.BloodTransfusionRepository
{
    public class BloodTransfusionRepository(SQLServerContext sqlServerContext) : IBloodTransfusionRepository
    {
        private readonly SQLServerContext _sqlServerContext = sqlServerContext;

        public async Task<IEnumerable<BloodTransfusion>> GetAllBloodTransfusionAsync()
        {
            return await _sqlServerContext.BloodTransfusions.AsNoTracking().ToListAsync();
        }

        public async Task<BloodTransfusion?> GetBloodTransfusionByIdAsync(Guid id)
        {
            return await _sqlServerContext.BloodTransfusions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddBloodTransfusionAsync(BloodTransfusion bloodTransfusion)
        {
            await _sqlServerContext.BloodTransfusions.AddAsync(bloodTransfusion);
            await _sqlServerContext.SaveChangesAsync();
        }

        public async Task UpdateBloodTransfusionAsync(BloodTransfusion bloodTransfusion)
        {
            _sqlServerContext.BloodTransfusions.Update(bloodTransfusion);
            await _sqlServerContext.SaveChangesAsync();
        }
    }
}

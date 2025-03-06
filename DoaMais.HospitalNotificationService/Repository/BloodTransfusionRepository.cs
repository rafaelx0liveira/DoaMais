using DoaMais.HospitalNotificationService.Model;
using DoaMais.HospitalNotificationService.Model.Context;
using DoaMais.HospitalNotificationService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.HospitalNotificationService.Repository
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

using DoaMais.StockService.Model;
using DoaMais.StockService.Model.Context;
using DoaMais.StockService.Repository.Interface;
using DoaMais.StockService.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.StockService.Repository
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

        public async Task AddBloodTransfusionAsync(BloodTransfusionVO bloodTransfusionVO)
        {
            var bloodTransfusion = new BloodTransfusion
            {
                HospitalId = bloodTransfusionVO.HospitalId,
                QuantityML = bloodTransfusionVO.QuantityML,
                BloodType = bloodTransfusionVO.BloodType,
                RHFactor = bloodTransfusionVO.RHFactor,
                TransfusionDate = bloodTransfusionVO.TransfusionDate
            };

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

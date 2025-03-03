using DoaMais.Domain.Entities;
using DoaMais.Domain.Interfaces.Repository.HospitalRepository;
using DoaMais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.Infrastructure.Repositories.HospitalRepository
{
    public class HospitalRepository(SQLServerContext sqlServerContext) : IHospitalRepository
    {
        private readonly SQLServerContext _sqlServerContext = sqlServerContext;

        public async Task<IEnumerable<Hospital>> GetAllHospitalAsync()
        {
            return await _sqlServerContext.Hospitals.Where(x => x.IsActive == true)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Hospital?> GetHospitalByIdAsync(Guid id)
        {
            return await _sqlServerContext.Hospitals.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Hospital?> GetHospitalByCNPJAsync(string cnpj)
        {
            return await _sqlServerContext.Hospitals.FirstOrDefaultAsync(h => h.CNPJ == cnpj);
        }

        public async Task AddHospitalAsync(Hospital hospital)
        {
            await _sqlServerContext.Hospitals.AddAsync(hospital);
            await _sqlServerContext.SaveChangesAsync();
        }

        public async Task<bool> HospitalExistsAsync(string cnpj)
        {
            return await _sqlServerContext.Hospitals.AnyAsync(x => x.CNPJ == cnpj);
        }

        public async Task UpdateHospitalAsync(Hospital hospital)
        {
            _sqlServerContext.Hospitals.Update(hospital);
            await _sqlServerContext.SaveChangesAsync();
        }
    }
}

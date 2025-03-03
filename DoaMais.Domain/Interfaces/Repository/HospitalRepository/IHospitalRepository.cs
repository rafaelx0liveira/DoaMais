using DoaMais.Domain.Entities;

namespace DoaMais.Domain.Interfaces.Repository.HospitalRepository
{
    public interface IHospitalRepository
    {
        Task<IEnumerable<Hospital>> GetAllHospitalAsync();
        Task<Hospital?> GetHospitalByIdAsync(Guid id);
        Task<Hospital?> GetHospitalByCNPJAsync(string cnpj);
        Task AddHospitalAsync(Hospital hospital);
        Task UpdateHospitalAsync(Hospital hospital);
        Task<bool> HospitalExistsAsync(string cnpj);
    }
}

using DoaMais.Domain.Entities;

namespace DoaMais.Domain.Interfaces.Repository.BloodTransfusionRepository
{
    public interface IBloodTransfusionRepository
    {
        Task<IEnumerable<BloodTransfusion>> GetAllBloodTransfusionsAsync();
    }
}

using DoaMais.StockService.Model;
using DoaMais.StockService.ValueObject;

namespace DoaMais.StockService.Repository.Interface
{
    public interface IBloodTransfusionRepository
    {
        Task<IEnumerable<BloodTransfusion>> GetAllBloodTransfusionAsync();

        Task<BloodTransfusion?> GetBloodTransfusionByIdAsync(Guid id);

        Task AddBloodTransfusionAsync(BloodTransfusionVO bloodTransfusionVO);

        Task UpdateBloodTransfusionAsync(BloodTransfusion bloodTransfusion);
    }
}

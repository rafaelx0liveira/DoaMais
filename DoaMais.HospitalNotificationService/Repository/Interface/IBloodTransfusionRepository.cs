using DoaMais.HospitalNotificationService.Model;

namespace DoaMais.HospitalNotificationService.Repository.Interface
{
    public interface IBloodTransfusionRepository
    {
        Task<IEnumerable<BloodTransfusion>> GetAllBloodTransfusionAsync();

        Task<BloodTransfusion?> GetBloodTransfusionByIdAsync(Guid id);

        Task AddBloodTransfusionAsync(BloodTransfusion bloodTransfusion);

        Task UpdateBloodTransfusionAsync(BloodTransfusion bloodTransfusion);
    }
}

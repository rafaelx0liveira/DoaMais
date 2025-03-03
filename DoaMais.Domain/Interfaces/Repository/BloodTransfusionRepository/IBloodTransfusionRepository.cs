using DoaMais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.Domain.Interfaces.Repository.BloodTransfusionRepository
{
    public interface IBloodTransfusionRepository
    {
        Task<IEnumerable<BloodTransfusion>> GetAllBloodTransfusionAsync();

        Task<BloodTransfusion?> GetBloodTransfusionByIdAsync(Guid id);

        Task AddBloodTransfusionAsync(BloodTransfusion bloodTransfusion);

        Task UpdateBloodTransfusionAsync(BloodTransfusion bloodTransfusion);
    }
}

using DoaMais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.Domain.Interfaces.Repository.DonorRepository
{
    public interface IDonorRepository
    {
        Task<Guid> AddDonorAsync(Donor donor);
        Task UpdateDonorAsync(Donor donor);
        Task<Donor> GetDonorByEmailAsync(string email);
        Task<IEnumerable<Donor>> GetAllDonorsAsync();
        Task<Donor> GetDonorByIdAsync(Guid id);
        Task<bool> DonorExistsAsync(string email);
    }
}

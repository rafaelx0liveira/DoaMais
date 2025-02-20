using DoaMais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.Domain.Interfaces.Repository.DonationRepository
{
    public interface IDonationRepository
    {
        Task<Guid> AddDonationAsync(Donation donation);
        Task UpdateDonationAsync(Donation donation);
        Task<Donation> GetDonationByDateAsync(DateTime date);
        Task<IEnumerable<Donation>> GetAllDonationsAsync();
        Task<Donation> GetDonationByIdAsync(Guid id);
        Task<Donation> GetLastDonationAsync(Guid id);
    }
}

using DoaMais.Domain.Entities;
using DoaMais.Domain.Interfaces.Repository.DonationRepository;
using DoaMais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.Infrastructure.Repositories.DonationRepository
{
    public class DonationRepository(SQLServerContext context)
        : IDonationRepository
    {
        private readonly SQLServerContext _context = context;

        public async Task<Guid> AddDonationAsync(Donation donation)
        {
            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();
            return donation.Id;
        }

        public async Task UpdateDonationAsync(Donation donation)
        {
            _context.Donations.Update(donation);
            await _context.SaveChangesAsync();
        }

        public async Task<Donation> GetDonationByDateAsync(DateTime date)
        {
            return await _context.Donations.FirstOrDefaultAsync(x => x.DonationDate == date);
        }

        public async Task<IEnumerable<Donation>> GetAllDonationsAsync()
        {
            return await _context.Donations
                .Include(d => d.Donor)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Donation> GetDonationByIdAsync(Guid id)
        {
            return await _context.Donations.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Donation> GetLastDonationAsync(Guid id)
        {
            return await _context.Donations
                .Include(d => d.Donor)
                    .ThenInclude(d => d.Address)
                .Where(d => d.DonorId == id)
                .OrderByDescending(d => d.DonationDate)
                .FirstOrDefaultAsync();
        }
    }
}

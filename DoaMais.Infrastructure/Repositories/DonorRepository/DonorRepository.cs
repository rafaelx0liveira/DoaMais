using DoaMais.Domain.Entities;
using DoaMais.Domain.Interfaces.Repository.DonorRepository;
using DoaMais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.Infrastructure.Repositories.DonorRepository
{
    public class DonorRepository(SQLServerContext context)
        : IDonorRepository
    {
        private readonly SQLServerContext _context = context;

        public async Task<Guid> AddDonorAsync(Donor donor)
        {
            _context.Donors.Add(donor);
            await _context.SaveChangesAsync();
            return donor.Id;
        }

        public async Task UpdateDonorAsync(Donor donor)
        {
            _context.Update(donor);
            await _context.SaveChangesAsync();
        }

        public async Task<Donor> GetDonorByEmailAsync(string email)
        {
            var donor = await _context.Donors.FirstOrDefaultAsync(x => x.Email == email);
            return donor;
        }

        public async Task<IEnumerable<Donor>> GetAllDonorsAsync()
        {
            var donors = await _context.Donors.AsNoTracking().ToListAsync();

            return donors;
        }

        public async Task<Donor> GetDonorByIdAsync(Guid id)
        {
            var donor = await _context.Donors.SingleOrDefaultAsync(x => x.Id == id);

            return donor;
        }

        public async Task<bool> DonorExistsAsync(string email)
        {
            var donorExists = await _context.Donors.AnyAsync(x => x.Email == email);

            return donorExists;
        }
    }
}

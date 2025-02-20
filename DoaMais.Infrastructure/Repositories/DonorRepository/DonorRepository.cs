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
            _context.Donors.Update(donor);
            await _context.SaveChangesAsync();
        }

        public async Task<Donor> GetDonorByEmailAsync(string email)
        {
            return await _context.Donors.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<IEnumerable<Donor>> GetAllDonorsAsync()
        {
            return await _context.Donors
                .Include(d => d.Address)
                .Include(d => d.Donations)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Donor> GetDonorByIdAsync(Guid id)
        {
            return await
                _context.Donors
                .Include(d => d.Address)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> DonorExistsAsync(string email)
        {
            return await _context.Donors.AnyAsync(x => x.Email == email);
        }
    }
}

using DoaMais.Domain.Entities;
using DoaMais.Domain.Interfaces.Repository.AddressRepository;
using DoaMais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.Infrastructure.Repositories.AddressRepository
{
    public class AddressRepository(SQLServerContext context) : IAddressRepository
    {
        private readonly SQLServerContext _context = context;

        public async Task AddAddressAsync(Address address)
        {
            await _context.Addresses.AddAsync(address);
        }

        public Task<Address> GetAddressAsync(Guid id)
        {
            return _context.Addresses.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<bool> AddressExistsAsync(Guid id)
        {
            return _context.Addresses.AnyAsync(x => x.Id == id);
        }

        public Task<Address> GetAddressPostalCodeAsync(string postalCode)
        {
            return _context.Addresses.FirstOrDefaultAsync(x => x.PostalCode == postalCode);
        }
    }
}

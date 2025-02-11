using DoaMais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.Domain.Interfaces.Repository.AddressRepository
{
    public interface IAddressRepository
    {
        Task<Address> GetAddressAsync(Guid id);
        Task<bool> AddressExistsAsync(Guid id);
        Task<Address> GetAddressPostalCodeAsync(string postalCode);

        Task AddAddressAsync(Address address);
    }
}

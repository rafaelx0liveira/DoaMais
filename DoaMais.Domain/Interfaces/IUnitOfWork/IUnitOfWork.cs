using DoaMais.Domain.Interfaces.Repository.AddressRepository;
using DoaMais.Domain.Interfaces.Repository.DonationRepository;
using DoaMais.Domain.Interfaces.Repository.DonorRepository;
using DoaMais.Domain.Interfaces.Repository.EmployeeRepository;

namespace DoaMais.Domain.Interfaces.IUnitOfWork
{
    public interface IUnitOfWork
    {
        IDonorRepository Donors { get; }
        IEmployeeRepository Employee { get; }
        IAddressRepository Address { get; }
        IDonationRepository Donation { get; }

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
    }
}

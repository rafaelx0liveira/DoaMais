using DoaMais.Domain.Interfaces.Repository.AddressRepository;
using DoaMais.Domain.Interfaces.Repository.DonationRepository;
using DoaMais.Domain.Interfaces.Repository.DonorRepository;
using DoaMais.Domain.Interfaces.Repository.EmployeeRepository;
using DoaMais.Domain.Interfaces.Repository.HospitalRepository;

namespace DoaMais.Domain.Interfaces.IUnitOfWork
{
    public interface IUnitOfWork
    {
        IDonorRepository Donors { get; }
        IEmployeeRepository Employee { get; }
        IAddressRepository Address { get; }
        IDonationRepository Donation { get; }
        IHospitalRepository Hospital { get; }

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
    }
}

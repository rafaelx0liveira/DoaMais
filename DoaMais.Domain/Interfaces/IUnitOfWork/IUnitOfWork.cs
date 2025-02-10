using DoaMais.Domain.Interfaces.Repository.DonorRepository;
using DoaMais.Domain.Interfaces.Repository.EmployeeRepository;

namespace DoaMais.Domain.Interfaces.UnityOfWork
{
    public interface IUnitOfWork
    {
        IDonorRepository Donors { get; }

        IEmployeeRepository Employee { get; }
        Task<int> CompleteAsync();

        Task BeginTransactionAsync();
        Task CommitAsync();
    }
}

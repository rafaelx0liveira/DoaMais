using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.Domain.Interfaces.Repository.AddressRepository;
using DoaMais.Domain.Interfaces.Repository.BloodTransfusionRepository;
using DoaMais.Domain.Interfaces.Repository.DonationRepository;
using DoaMais.Domain.Interfaces.Repository.DonorRepository;
using DoaMais.Domain.Interfaces.Repository.EmployeeRepository;
using DoaMais.Domain.Interfaces.Repository.HospitalRepository;
using DoaMais.Domain.Interfaces.Repository.ReportRepository;
using DoaMais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Storage;


namespace DoaMais.Infrastructure.Persistence
{
    public class UnitOfWork(
        SQLServerContext sqlServerContext, 
        IDonorRepository donorRepository,
        IEmployeeRepository employeeRepository,
        IAddressRepository addressRepository,
        IDonationRepository donationRepository,
        IHospitalRepository hospitalRepository,
        IBloodTransfusionRepository bloodTransfusionRepository,
        IReportRepository reportRepository
    ) : IUnitOfWork
    {
        private readonly SQLServerContext _sqlServerContext = sqlServerContext;
        private readonly IDonorRepository _donorRepository = donorRepository;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly IAddressRepository _addressRepository = addressRepository;
        private readonly IDonationRepository _donationRepository = donationRepository;
        private readonly IHospitalRepository _hospitalRepository = hospitalRepository;
        private readonly IBloodTransfusionRepository _bloodTransfusionRepository = bloodTransfusionRepository;
        private readonly IReportRepository _reportRepository = reportRepository;
        private IDbContextTransaction? _transaction;

        public IDonorRepository Donors => _donorRepository;
        public IEmployeeRepository Employee => _employeeRepository;
        public IAddressRepository Address => _addressRepository;
        public IDonationRepository Donation => _donationRepository;
        public IHospitalRepository Hospital => _hospitalRepository;
        public IBloodTransfusionRepository BloodTransfusion => _bloodTransfusionRepository;
        public IReportRepository Report => _reportRepository;

        public async Task<int> CompleteAsync()
        {
            return await _sqlServerContext.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null) return; // Evita sobrescrever uma transação existente
            _transaction = await _sqlServerContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Nenhuma transação ativa.");

            try
            {
                await _sqlServerContext.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _sqlServerContext.Dispose();
                _transaction?.Dispose();
            }
        }
    }
}

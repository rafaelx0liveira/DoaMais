using DoaMais.Domain.Entities;
using DoaMais.Domain.Interfaces.Repository.EmployeeRepository;
using DoaMais.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.Infrastructure.Repositories.EmployeeRepository
{
    public class EmployeeRepository (SQLServerContext SqlServerContext) : IEmployeeRepository
    {
        private readonly SQLServerContext _context = SqlServerContext;

        public async Task<Guid> AddEmployeeAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return employee.Id;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(d => d.Address)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
            return await _context.Employees.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Employee?> GetEmployeeByIdAsync(Guid id)
        {
            return await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> EmployeeExists(string email)
        {
            return await _context.Employees.AnyAsync(x => x.Email == email);
        }
    }
}

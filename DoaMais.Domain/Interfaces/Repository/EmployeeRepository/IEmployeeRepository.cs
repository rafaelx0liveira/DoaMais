using DoaMais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.Domain.Interfaces.Repository.EmployeeRepository
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetEmployeeByEmailAsync(string email);
        Task<Employee?> GetEmployeeByIdAsync(Guid id);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Guid> AddEmployeeAsync(Employee employee);
        Task<bool> EmployeeExists(string email);
    }
}

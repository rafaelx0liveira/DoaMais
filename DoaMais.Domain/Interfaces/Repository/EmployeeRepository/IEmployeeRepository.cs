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
        Task<Employee> GetByEmailAsync(string email);
        Task<Employee> GetByIdAsync(Guid id);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Guid> AddAsync(Employee employee);
        Task<bool> EmployeeExists(string email);
    }
}

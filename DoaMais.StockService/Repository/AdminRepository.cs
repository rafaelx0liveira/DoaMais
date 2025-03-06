using DoaMais.StockService.DTOs;
using DoaMais.StockService.Model.Context;
using DoaMais.StockService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace DoaMais.StockService.Repository
{
    public class AdminRepository(SQLServerContext context) : IAdminRepository
    {
        private readonly SQLServerContext _context = context;

        public async Task<List<AdminDTO>> GetAdministratorsAsync()
        {
            return await _context.Employees
                .Where(e => e.Role == "Admin") 
                    .Select(e => new AdminDTO
                    {
                        Name = e.Name,
                        Email = e.Email
                    })
                .ToListAsync();
        }
    }
}

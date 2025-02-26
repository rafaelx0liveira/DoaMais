using DoaMais.StockService.DTOs;

namespace DoaMais.StockService.Repository.Interface
{
    public interface IAdminRepository
    {
        Task<List<AdminDTO>> GetAdministratorsAsync();
    }
}

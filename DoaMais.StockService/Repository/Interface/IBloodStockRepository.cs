using DoaMais.StockService.Model;
using DoaMais.StockService.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.StockService.Repository.Interface
{
    public interface IBloodStockRepository
    {
        Task AddBloodToStockAsync(BloodStock blood);
        Task<BloodStock> GetBloodByRHAndTypeAsync(BloodType type, RHFactor rhFactor);
        Task UpdateQuantityFromStockAsync(BloodStock bloodStock);
    }
}

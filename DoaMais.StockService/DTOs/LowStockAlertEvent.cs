using DoaMais.StockService.Model.Enums;

namespace DoaMais.StockService.DTOs
{
    public class LowStockAlertEvent
    {
        public BloodType BloodType { get; set; }
        public RHFactor RHFactor { get; set; }
        public int QuantityML { get; set; }

        public LowStockAlertEvent(BloodType bloodType, RHFactor rHFactor, int quantityML)
        {
            BloodType = bloodType;
            RHFactor = rHFactor;
            QuantityML = quantityML;
        }
    }
}

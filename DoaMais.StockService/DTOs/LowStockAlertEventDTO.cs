using DoaMais.StockService.Model.Enums;
using System.Text.Json.Serialization;

namespace DoaMais.StockService.DTOs
{
    public class LowStockAlertEventDTO
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BloodType BloodType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RHFactor RHFactor { get; set; }

        public int QuantityML { get; set; }
        public List<AdminDTO> AdminEmails { get; set; }

        public LowStockAlertEventDTO(BloodType bloodType, RHFactor rHFactor, int quantityML, List<AdminDTO> adminEmails)
        {
            BloodType = bloodType;
            RHFactor = rHFactor;
            QuantityML = quantityML;
            AdminEmails = adminEmails;
        }
    }
}

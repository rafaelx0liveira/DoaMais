using DoaMais.LowStockAlertService.Utils;
using DoaMais.LowStockAlertService.ValueObject.Enums;
using System.Text.Json.Serialization;

namespace DoaMais.LowStockAlertService.ValueObject
{
    public class LowStockAlertEventVO
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BloodType BloodType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RHFactor RHFactor { get; set; }

        public int QuantityML { get; set; }

        [JsonConverter(typeof(AdminListJsonConverter))]
        public List<Admin> AdminEmails { get; set; }

        public LowStockAlertEventVO(BloodType bloodType, RHFactor rHFactor, int quantityML, List<Admin> adminEmails)
        {
            BloodType = bloodType;
            RHFactor = rHFactor;
            QuantityML = quantityML;
            AdminEmails = adminEmails;
        }
    }
}

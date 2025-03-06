using DoaMais.StockService.Model.Enums;
using System.Text.Json.Serialization;

namespace DoaMais.StockService.ValueObject
{
    public class BloodTransfusionVO
    {
        public Guid HospitalId { get; set; }
        public int QuantityML { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BloodType BloodType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RHFactor RHFactor { get; set; }

        public DateTime TransfusionDate { get; set; }

        public BloodTransfusionVO(Guid hospitalId, int quantity, BloodType bloodType, RHFactor rhFactor)
        {
            HospitalId = hospitalId;
            QuantityML = quantity;
            BloodType = bloodType;
            RHFactor = rhFactor;
            TransfusionDate = DateTime.UtcNow;
        }
    }
}

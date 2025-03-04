using DoaMais.StockService.Model.Enums;
using System.Text.Json.Serialization;

namespace DoaMais.StockService.DTOs
{
    public class DonationRegisteredEventDTO
    {
        public Guid DonorId { get; set; }
        public string DonorName { get; set; }
        public string DonorEmail { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BloodType BloodType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RHFactor RHFactor { get; set; }
        public int Quantity { get; set; }

        public DonationRegisteredEventDTO(Guid donorId, string donorName, string donorEmail, BloodType bloodType, RHFactor rhFactor, int quantity)
        {
            DonorId = donorId;
            DonorName = donorName;
            DonorEmail = donorEmail;
            BloodType = bloodType;
            RHFactor = rhFactor;
            Quantity = quantity;
        }
    }
}

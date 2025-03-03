using DoaMais.MessageBus.Model;
using DoaMais.StockService.Model.Enums;
using System.Text.Json.Serialization;

namespace DoaMais.StockService.ValueObject
{
    public class DonationRegisteredEventVO : BaseMessage
    {
        public Guid DonorId { get; set; }
        public string DonorName { get; set; }
        public string DonorEmail { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BloodType BloodType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RHFactor RHFactor { get; set; }
        public int Quantity { get; set; }

        public DonationRegisteredEventVO()
        {
            
        }

        public DonationRegisteredEventVO(Guid donorId, string donorName, string donorEmail, BloodType bloodType, RHFactor rHFactor, int quantity, DateTime date) : base()
        {
            DonorId = donorId;
            DonorName = donorName;
            DonorEmail = donorEmail;
            BloodType = bloodType;
            RHFactor = rHFactor;
            Quantity = quantity;
        }
    }
}

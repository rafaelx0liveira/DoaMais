using DoaMais.Domain.Entities.Enums;
using System.Text.Json.Serialization;

namespace DoaMais.Application.DTOs
{
    public class BloodTransfusionRequestedEventDTO
    {
        public Guid HospitalId { get; set; }
        public int QuantityML { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BloodType BloodType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RHFactor RHFactor { get; set; }

        public BloodTransfusionRequestedEventDTO(Guid hospitalId, int quantityML, BloodType bloodType, RHFactor rhFactor)
        {
            HospitalId = hospitalId;
            QuantityML = quantityML;
            BloodType = bloodType;
            RHFactor = rhFactor;
        }
    }

}

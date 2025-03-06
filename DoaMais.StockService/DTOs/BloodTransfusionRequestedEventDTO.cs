using DoaMais.StockService.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DoaMais.StockService.DTOs
{
    public class BloodTransfusionRequestedEventDTO
    {
        public Guid HospitalId { get; set; }
        public string HospitalName { get; set; }
        public string HospitalEmail { get; set; }
        public int QuantityML { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BloodType BloodType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RHFactor RHFactor { get; set; }

        public BloodTransfusionRequestedEventDTO(Guid hospitalId, string hospitalName, string hospitalEmail, int quantityML, BloodType bloodType, RHFactor rhFactor)
        {
            HospitalId = hospitalId;
            HospitalName = hospitalName;
            HospitalEmail = hospitalEmail;
            QuantityML = quantityML;
            BloodType = bloodType;
            RHFactor = rhFactor;
        }
    }
}

﻿using DoaMais.StockService.Model.Enums;
using System.Text.Json.Serialization;

namespace DoaMais.StockService.DTOs
{
    public class BloodTransfusionNotificationEventDTO
    {
        public Guid HospitalId { get; set; } 

        public string HospitalName { get; set; }
        public string HospitalEmail { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BloodType BloodType { get; set; } 

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RHFactor RHFactor { get; set; } 
        public int QuantityML { get; set; } 
        public string Status { get; set; } 
        public DateTime TransfusionDate { get; set; }

        public BloodTransfusionNotificationEventDTO(Guid hospitalId, string hospitalName, string hospitalEmail, BloodType bloodType, RHFactor rhFactor, int quantityML, string status)
        {
            HospitalId = hospitalId;
            HospitalName = hospitalName;
            HospitalEmail = hospitalEmail;
            BloodType = bloodType;
            RHFactor = rhFactor;
            QuantityML = quantityML;
            Status = status;
            TransfusionDate = DateTime.UtcNow;
        }
    }

}

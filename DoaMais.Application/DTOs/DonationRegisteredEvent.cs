﻿using DoaMais.Domain.Entities.Enums;
using DoaMais.MessageBus.Model;
using System.Text.Json.Serialization;

namespace DoaMais.Application.DTOs
{
    public class DonationRegisteredEvent : BaseMessage
    {
        public Guid DonorId { get; set; }
        public string DonorEmail { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BloodType BloodType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RHFactor RHFactor { get; set; }
        public int Quantity { get; set; }

        public DonationRegisteredEvent(Guid donorId, string donorEmail, BloodType bloodType, RHFactor rhFactor, int quantity)
            : base() // Constructor for BaseMessage
        {
            DonorId = donorId;
            DonorEmail = donorEmail;
            BloodType = bloodType;
            RHFactor = rhFactor;
            Quantity = quantity;
        }
    }
}

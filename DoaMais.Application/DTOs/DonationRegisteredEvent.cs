using DoaMais.Domain.Entities.Enums;
using DoaMais.MessageBus.Model;

namespace DoaMais.Application.DTOs
{
    public class DonationRegisteredEvent : BaseMessage
    {
        public Guid DonorId { get; set; }
        public string DonorEmail { get; set; }
        public BloodType BloodType { get; set; }
        public decimal Quantity { get; private set; }
        public DateTime Date { get; set; }

        public DonationRegisteredEvent(Guid donorId, string donorEmail, BloodType bloodType, decimal quantity)
            : base() // Constructor for BaseMessage
        {
            DonorId = donorId;
            DonorEmail = donorEmail;
            BloodType = bloodType;
            Quantity = quantity;
            Date = DateTime.UtcNow;
        }
    }
}

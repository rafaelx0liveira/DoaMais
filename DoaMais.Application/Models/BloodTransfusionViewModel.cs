using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;

namespace DoaMais.Application.Models
{
    public class BloodTransfusionViewModel
    {
        public Guid BloodTransfusionId { get; set; }
        public Hospital Hospital { get; set; }

        public int QuantityML { get; set; }
        public BloodType BloodType { get; set; }
        public RHFactor RHFactor { get; set; }
        public DateTime TransfusionDate { get; set; }

        public BloodTransfusionViewModel(Guid bloodTransfusionId, Hospital hospital, int quantityML, BloodType bloodType, RHFactor rhFactor, DateTime transfusionDate)
        {
            BloodTransfusionId = bloodTransfusionId;
            Hospital = hospital;
            QuantityML = quantityML;
            BloodType = bloodType;
            RHFactor = rhFactor;
            TransfusionDate = transfusionDate;
        }

        public static BloodTransfusionViewModel FromEntity(BloodTransfusion bloodTransfusion)
        {
            return new BloodTransfusionViewModel
            (
                bloodTransfusion.Id,
                new Hospital
                {
                    Id = bloodTransfusion.HospitalId,
                    Name = bloodTransfusion.Hospital.Name,
                    CNPJ = bloodTransfusion.Hospital.CNPJ,
                    Phone = bloodTransfusion.Hospital.Phone,
                    Email = bloodTransfusion.Hospital.Email,
                },
                bloodTransfusion.QuantityML,
                bloodTransfusion.BloodType,
                bloodTransfusion.RHFactor,
                bloodTransfusion.TransfusionDate
            );
        }
    }
}


using DoaMais.Domain.Entities.Base;
using DoaMais.Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoaMais.Domain.Entities
{
    public class BloodTransfusion : BaseEntity
    {
        [ForeignKey("Hospital")]
        public Guid HospitalId { get; set; }
        public virtual Hospital Hospital { get; set; }

        public int QuantityML { get; set; }
        public BloodType BloodType { get; set; }
        public RHFactor RHFactor { get; set; }
        public DateTime TransfusionDate { get; set; }
    }
}

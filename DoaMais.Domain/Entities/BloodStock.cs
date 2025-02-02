using DoaMais.Domain.Entities.Base;
using DoaMais.Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoaMais.Domain.Entities
{
    public class BloodStock : BaseEntity
    {
        [Column("BloodType")]
        public BloodType BloodType { get; set; }

        [Column("RHFactor")]
        public RHFactor RHFactor { get; set; }

        [Column("QuantityML")]
        public int QuantityML { get; set; }
    }
}

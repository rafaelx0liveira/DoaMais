using DoaMais.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoaMais.Domain.Entities
{
    public class Donation : BaseEntity
    {
        [ForeignKey("Donor")]
        public long DonorId { get; set; }

        public virtual Donor Donor { get; set; }

        [Column("DonationDate")]
        public DateTime DonationDate { get; set; }

        [Column("QuantityML")]
        public int QuantityML { get; set; }
    }
}

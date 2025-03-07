using DoaMais.ReportService.Model.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.ReportService.Model
{
    public class Donation : BaseEntity
    {
        [ForeignKey("Donor")]
        public Guid DonorId { get; set; }

        public virtual Donor Donor { get; set; }

        [Column("DonationDate")]
        public DateTime DonationDate { get; set; }

        [Column("QuantityML")]
        public int QuantityML { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DoaMais.ReportService.Model.Base;

namespace DoaMais.ReportService.Model
{
    public class Address : BaseEntity
    {
        [Column("StreetAddress")]
        [StringLength(300)]
        public string? StreetAddress { get; set; }

        [Column("City")]
        [StringLength(300)]
        public string? City { get; set; }

        [Column("State")]
        [StringLength(100)]
        public string? State { get; set; }

        [Column("PostalCode")]
        [StringLength(50)]
        public string? PostalCode { get; set; }

        public virtual ICollection<Donor> Donors { get; set; } = new List<Donor>();
    }
}

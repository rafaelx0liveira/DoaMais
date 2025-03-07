using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DoaMais.ReportService.Model.Base;
using DoaMais.ReportService.Model.Enums;

namespace DoaMais.ReportService.Model
{
    public class Donor : BaseEntity
    {
        [ForeignKey("Address")]
        public Guid AddressId { get; set; }

        public virtual Address Address { get; set; }

        [Column("Name")]
        [StringLength(200)]
        public string? Name { get; set; }

        [Column("Email")]
        [EmailAddress]
        public string? Email { get; set; }

        [Column("DateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [Column("BiologicalSex")]
        public BiologicalSex BiologicalSex { get; set; }

        [Column("Weight")]
        [Range(0, double.MaxValue)]
        public Decimal Weight { get; set; }

        [Column("BloodType")]
        public BloodType BloodType { get; set; }

        [Column("RHFactor")]
        public RHFactor RHFactor { get; set; }

        public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();
    }
}

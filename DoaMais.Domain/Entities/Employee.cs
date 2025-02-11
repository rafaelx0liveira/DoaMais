using DoaMais.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DoaMais.Domain.Entities.Enums;

namespace DoaMais.Domain.Entities
{
    public class Employee : BaseEntity
    {
        [Column("Name")]
        [StringLength(200)]
        public string Name { get; set; }

        [Column("Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Column("PasswordHash")]
        public string PasswordHash { get; set; } 

        [Column("Role")]
        public EmployeeRole Role { get; set; }

        [ForeignKey("Address")]
        public Guid AddressId { get; set; }

        public virtual Address Address { get; set; }

    }
}

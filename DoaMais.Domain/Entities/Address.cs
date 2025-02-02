using DoaMais.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.Domain.Entities
{
    public class Address : BaseEntity
    {
        [Column("StreetAddress")]
        [StringLength(300)]
        public string? StreetAddres { get; set; }

        [Column("City")]
        [StringLength(300)]
        public string? City { get; set; }

        [Column("State")]
        [StringLength(100)]
        public string? State { get; set; }

        [Column("PostalCode")]
        [StringLength(50)]
        public string? PostalCode { get; set; }
    }
}

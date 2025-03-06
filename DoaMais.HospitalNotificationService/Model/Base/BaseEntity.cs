using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.HospitalNotificationService.Model.Base
{
    public class BaseEntity
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }
    }
}

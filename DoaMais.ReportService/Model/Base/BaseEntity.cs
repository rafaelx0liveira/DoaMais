using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoaMais.ReportService.Model.Base
{
    public class BaseEntity
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }
    }
}

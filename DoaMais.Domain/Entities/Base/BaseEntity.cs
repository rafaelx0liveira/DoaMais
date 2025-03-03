using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoaMais.Domain.Entities.Base
{
    public class BaseEntity
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }
    }
}

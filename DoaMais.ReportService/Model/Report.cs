using DoaMais.ReportService.Model.Base;
using DoaMais.ReportService.Model.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoaMais.ReportService.Model
{
    public class Report : BaseEntity
    {
        [Column("Name")]
        public string Name { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("FileData")]
        public byte[] FileData { get; set; }

        [Column("Type")]
        public ReportType ReportType { get; set; }
    }
}

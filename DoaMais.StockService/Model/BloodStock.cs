using DoaMais.StockService.Model.Base;
using DoaMais.StockService.Model.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoaMais.StockService.Model
{
    public class BloodStock : BaseEntity
    {
        [Column("BloodType")]
        public BloodType BloodType { get; set; }

        [Column("RHFactor")]
        public RHFactor RHFactor { get; set; }

        [Column("QuantityML")]
        public int QuantityML { get; set; }
    }
}

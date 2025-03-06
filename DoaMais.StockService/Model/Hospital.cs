using DoaMais.StockService.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.StockService.Model
{
    public class Hospital : BaseEntity
    {
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<BloodTransfusion> BloodTransfusions { get; set; } = new List<BloodTransfusion>();
    }
}

using DoaMais.Domain.Entities.Base;

namespace DoaMais.Domain.Entities
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

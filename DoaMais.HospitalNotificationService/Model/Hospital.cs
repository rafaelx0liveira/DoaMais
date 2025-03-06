using DoaMais.HospitalNotificationService.Model.Base;

namespace DoaMais.HospitalNotificationService.Model
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

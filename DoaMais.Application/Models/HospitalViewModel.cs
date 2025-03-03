using DoaMais.Domain.Entities;

namespace DoaMais.Application.Models
{
    public class HospitalViewModel
    {
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public HospitalViewModel(string name, string cnpj, string email, string phone)
        {
            Name = name;
            CNPJ = cnpj;
            Email = email;
            Phone = phone;
        }

        public static HospitalViewModel FromEntity(Hospital hospital)
        {
            return new HospitalViewModel(
                hospital.Name,
                hospital.CNPJ,
                hospital.Email,
                hospital.Phone
            );
        }
    }
}

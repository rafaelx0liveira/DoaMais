using DoaMais.Application.Models;
using DoaMais.Domain.Entities;
using MediatR;

namespace DoaMais.Application.Commands.HospitalCommands.CreateHospitalCommand
{
    public class CreateHospitalCommand : IRequest<ResultViewModel<Guid>>
    {
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; private set; }

        public CreateHospitalCommand(string name, string cnpj, string email, string phone)
        {
            Name = name;
            CNPJ = cnpj;
            Email = email;
            Phone = phone;
            IsActive = true;
        }

        public Hospital ToEntity()
        {
            return new Hospital
            {
                Name = Name,
                CNPJ = CNPJ,
                Email = Email,
                Phone = Phone,
                IsActive = IsActive
            };
        }
    }
}

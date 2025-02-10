using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;
using MediatR;

namespace DoaMais.Application.Commands.DonorCommands.CreateDonorCommand
{
    public class CreateDonorCommand : IRequest<ResultViewModel<Guid>>
    {
        public CreateDonorCommand(DonorDTO donorDTO)
        {
            DonorDTO = donorDTO;
        }

        public DonorDTO DonorDTO { get; }

        public Donor ToEntity()
        {
            var address = new Address
            {
                StreetAddress = DonorDTO.Address.StreetAddress,
                City = DonorDTO.Address.City,
                State = DonorDTO.Address.State,
                PostalCode = DonorDTO.Address.PostalCode
            };

            return new Donor
            {
                Name = DonorDTO.Name,
                Email = DonorDTO.Email,
                DateOfBirth = DonorDTO.DateOfBirth,
                Gender = Enum.Parse<Gender>(DonorDTO.Gender),
                Weight = (decimal)DonorDTO.Weight,
                BloodType = Enum.Parse<BloodType>(DonorDTO.BloodType),
                RHFactor = Enum.Parse<RHFactor>(DonorDTO.RhFactor),
                Address = address
            };
        }
    }

}

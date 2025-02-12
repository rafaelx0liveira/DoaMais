using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Application.Validators;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace DoaMais.Application.Commands.DonorCommands.CreateDonorCommand
{
    public class CreateDonorCommand : IRequest<ResultViewModel<Guid>>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }

        [JsonConverter(typeof(InvalidEnumConverter<Gender>))]
        public Gender Gender { get; set; }
        public decimal Weight { get; set; }

        [JsonConverter(typeof(InvalidEnumConverter<BloodType>))]
        public BloodType BloodType { get; set; }

        [JsonConverter(typeof(InvalidEnumConverter<RHFactor>))]
        public RHFactor RhFactor { get; set; }

        public AddressDTO Address { get; set; }


        public CreateDonorCommand(string name, string email, DateTime dateOfBirth, Gender gender, decimal weight, BloodType bloodType, RHFactor rhFactor, AddressDTO address)
        {
            Name = name;
            Email = email;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Weight = weight;
            BloodType = bloodType;
            RhFactor = rhFactor;
            Address = address;
        }

        public Donor ToEntity()
        {
            var address = new Address
            {
                StreetAddress = Address.StreetAddress,
                City = Address.City,
                State = Address.State,
                PostalCode = Address.PostalCode
            };

            return new Donor
            {
                Name = Name,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender,
                Weight = Weight,
                BloodType = BloodType,
                RHFactor = RhFactor,
                Address = address
            };
        }
    }

}

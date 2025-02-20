using DoaMais.Application.DTOs;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;

namespace DoaMais.Application.Models
{
    public class DonorViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public BiologicalSex BiologicalSex { get; private set; }
        public decimal Weight { get; private set; }
        public BloodType BloodType { get; private set; }
        public RHFactor RhFactor { get; private set; }

        public AddressDTO Address { get; private set; }

        public DonorViewModel(Guid id, string name, string email, DateTime dateOfBirth, BiologicalSex biologicalSex, decimal weight, BloodType bloodType, RHFactor rhFactor, AddressDTO address)
        {
            Id = id;
            Name = name;
            Email = email;
            DateOfBirth = dateOfBirth;
            BiologicalSex = biologicalSex;
            Weight = weight;
            BloodType = bloodType;
            RhFactor = rhFactor;
            Address = address;
        }

        public static DonorViewModel FromEntity(Donor donor) 
        {
            return new DonorViewModel(
                donor.Id,
                donor.Name,
                donor.Email,
                donor.DateOfBirth,
                donor.BiologicalSex,
                donor.Weight,
                donor.BloodType,
                donor.RHFactor,
                new AddressDTO
                {
                    StreetAddress = donor.Address.StreetAddress,
                    City = donor.Address.City,
                    State = donor.Address.State,
                    PostalCode = donor.Address.PostalCode
                }
            ); 
        }
    }
}

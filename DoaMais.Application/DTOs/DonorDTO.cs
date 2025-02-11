
using DoaMais.Domain.Entities.Enums;

namespace DoaMais.Application.DTOs
{
    public class DonorDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public decimal Weight { get; set; }
        public BloodType BloodType { get; set; }
        public RHFactor RhFactor { get; set; }

        public AddressDTO Address { get; set; }
    }

}

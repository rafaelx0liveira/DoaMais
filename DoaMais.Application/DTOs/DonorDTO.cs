
namespace DoaMais.Application.DTOs
{
    public class DonorDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public double Weight { get; set; }
        public string BloodType { get; set; }
        public string RhFactor { get; set; }

        public AddressDTO Address { get; set; }
    }

}

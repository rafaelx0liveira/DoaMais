using DoaMais.Application.DTOs;
using DoaMais.Domain.Entities;

namespace DoaMais.Application.Models
{
    public class DonationViewModel
    {
        public Guid DonationId { get; set; }
        public DateTime DonationDate { get; set; }
        public int QuantityML { get; set; }
        public Donor Donor { get; set; }

        public DonationViewModel(Guid donationId, DateTime donationDate, int quantityML, Donor donor)
        {
            DonationId = donationId;
            DonationDate = donationDate;
            QuantityML = quantityML;
            Donor = donor;
        }

        public static DonationViewModel FromEntity(Donation donation)
        {
            return new DonationViewModel
            (
                donation.Id,
                donation.DonationDate,
                donation.QuantityML,
                new Donor
                {
                    Id = donation.DonorId,
                    Name = donation.Donor.Name,
                    Email = donation.Donor.Email,
                    Address = new Address
                    {
                        StreetAddress = donation.Donor.Address.StreetAddress,
                        City = donation.Donor.Address.City,
                        State = donation.Donor.Address.State,
                        PostalCode = donation.Donor.Address.PostalCode
                    }
                }
              
            );
        }
    }
}

using DoaMais.Application.Models;
using DoaMais.Domain.Entities;
using MediatR;

namespace DoaMais.Application.Commands.DonationCommands.CreateDonationCommand
{
    public class CreateDonationCommand : IRequest<ResultViewModel<Guid>>
    {
        public Guid DonorId { get; set; }
        public DateTime DonationDate { get; private set; }
        public int QuantityML { get; set; }

        public CreateDonationCommand(Guid donorId, int quantityML)
        {
            DonorId = donorId;
            DonationDate = DateTime.UtcNow;
            QuantityML = quantityML;
        }

        public Donation ToEntity()
        {
            return new Donation
            {
                DonorId = DonorId,
                DonationDate = DonationDate,
                QuantityML = QuantityML
            };
        }
    }
}

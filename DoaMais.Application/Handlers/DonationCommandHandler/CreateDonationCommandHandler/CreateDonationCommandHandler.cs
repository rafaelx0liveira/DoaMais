using DoaMais.Application.Commands.DonationCommands.CreateDonationCommand;
using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Application.Services.DonorService;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.MessageBus.Interface;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace DoaMais.Application.Handlers.DonationCommandHandler.CreateDonationCommandHandler
{
    public class CreateDonationCommandHandler(
            IUnitOfWork unitOfWork,
            IMessageBus messageBus,
            IConfiguration configuration
        )
        : IRequestHandler<CreateDonationCommand, ResultViewModel<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMessageBus _messageBus = messageBus;
        private readonly IConfiguration _configuration = configuration;

        public async Task<ResultViewModel<Guid>> Handle(CreateDonationCommand request, CancellationToken cancellationToken)
        {
            var donor = await _unitOfWork.Donors.GetDonorByIdAsync(request.DonorId);

            if (donor == null) ResultViewModel<Guid>.Error($"Donor with Id {request.DonorId} was not found.");

            var lastDonation = await _unitOfWork.Donation.GetLastDonationAsync(request.DonorId);

            if (!DonorService.CanDonate(donor.DateOfBirth, donor.BiologicalSex, lastDonation?.DonationDate))
                return ResultViewModel<Guid>.Error($"The donor with Id {donor.Id} cannot make donation");

            var donation = request.ToEntity();

            await _unitOfWork.Donation.AddDonationAsync(donation);
            await _unitOfWork.CompleteAsync();

            var donationEvent = new DonationRegisteredEvent(  
                donation.DonorId,
                donation.Donor.Email,
                donation.Donor.BloodType,
                donation.Donor.RHFactor,
                donation.QuantityML);

            var donationQueueName = _configuration["RabbitMQ:DonationQueueName"] ?? throw new ArgumentNullException("DonationQueueName not found.");
            var donationExchangeName = _configuration["RabbitMQ:ExchangeDonationName"] ?? throw new ArgumentNullException("ExchangeDonationName not found.");

            await _messageBus.PublishMessageAsync(donationExchangeName, donationQueueName, donationEvent);

            return ResultViewModel<Guid>.Success(donation.Id);
        }
    }
}

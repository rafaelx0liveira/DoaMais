using DoaMais.Application.Commands.DonationCommands.CreateDonationCommand;
using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Application.Services.DonorService;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.MessageBus.Interface;
using MediatR;
using Microsoft.Extensions.Configuration;
using VaultService.Interface;

namespace DoaMais.Application.Handlers.DonationCommandHandler.CreateDonationCommandHandler
{
    public class CreateDonationCommandHandler(
            IUnitOfWork unitOfWork,
            IMessageBus messageBus,
            IConfiguration configuration,
            IVaultClient vaultClient
        )
        : IRequestHandler<CreateDonationCommand, ResultViewModel<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMessageBus _messageBus = messageBus;
        private readonly IConfiguration _configuration = configuration;
        private readonly IVaultClient _vaultClient = vaultClient;

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

            var donationEvent = new DonationRegisteredEventDTO(  
                donation.DonorId,
                donation.Donor.Name,
                donation.Donor.Email,
                donation.Donor.BloodType,
                donation.Donor.RHFactor,
                donation.QuantityML);

            var donationQueueName = _vaultClient.GetSecret(_configuration["RabbitMQ:DonationQueueName"]) ?? throw new ArgumentNullException("DonationQueueName not found.");

            var stockEventExchangeName = _vaultClient.GetSecret(_configuration["RabbitMQ:StockEventsExchangeName"]) ?? throw new ArgumentNullException("StockEventsExchangeName not found.");

            var donationRoutingKey = _vaultClient.GetSecret(_configuration["RabbitMQ:DonationRoutingKey"]) ?? throw new ArgumentNullException("DonationRoutingKey not found.");

            await _messageBus.PublishDirectMessageAsync(stockEventExchangeName, donationQueueName, donationRoutingKey, donationEvent);

            return ResultViewModel<Guid>.Success(donation.Id);
        }
    }
}

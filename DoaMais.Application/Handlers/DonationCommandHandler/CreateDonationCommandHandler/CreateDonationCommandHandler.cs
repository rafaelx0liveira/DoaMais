using DoaMais.Application.Commands.DonationCommands.CreateDonationCommand;
using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Application.Services.DonorService;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.MessageBus.Interface;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;
using VaultService.Interface;

namespace DoaMais.Application.Handlers.DonationCommandHandler.CreateDonationCommandHandler
{
    public class CreateDonationCommandHandler(
            IUnitOfWork unitOfWork,
            IMessageBus messageBus,
            IConfiguration configuration,
            IVaultClient vaultClient,
            ILogger logger
        )
        : IRequestHandler<CreateDonationCommand, ResultViewModel<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMessageBus _messageBus = messageBus;
        private readonly IConfiguration _configuration = configuration;
        private readonly IVaultClient _vaultClient = vaultClient;
        private readonly ILogger _logger = logger;

        public async Task<ResultViewModel<Guid>> Handle(CreateDonationCommand request, CancellationToken cancellationToken)
        {
            var donor = await _unitOfWork.Donors.GetDonorByIdAsync(request.DonorId);

            if (donor == null)
            {
                _logger.Warning($"Donor with Id {request.DonorId} was not found.");
                return ResultViewModel<Guid>.Error($"Donor with Id {request.DonorId} was not found.");
            }

            var lastDonation = await _unitOfWork.Donation.GetLastDonationAsync(request.DonorId);

            if (!DonorService.CanDonate(donor.DateOfBirth, donor.BiologicalSex, lastDonation?.DonationDate))
            {
                _logger.Warning($"The donor with Id {donor.Id} cannot make donation");
                return ResultViewModel<Guid>.Error($"The donor with Id {donor.Id} cannot make donation");
            }

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

            var donationQueueName = _configuration["KeyVaultSecrets:RabbitMQ:DonationQueue"] ?? throw new ArgumentNullException("DonationQueueName not found.");
            var donationQueueNameSecret = _vaultClient.GetSecret(donationQueueName);

            var stockExchangeName = _configuration["KeyVaultSecrets:RabbitMQ:StockEventsExchange"] ?? throw new ArgumentNullException("StockEventsExchangeName not found.");
            var stockEventExchangeNameSecret = _vaultClient.GetSecret(stockExchangeName);

            var donationRoutingKey = _configuration["KeyVaultSecrets:RabbitMQ:DonationRoutingKey"] ?? throw new ArgumentNullException("DonationRoutingKey not found.");
            var donationRoutingKeySecret = _vaultClient.GetSecret(donationRoutingKey);

            await _messageBus.PublishDirectMessageAsync(stockEventExchangeNameSecret, donationQueueNameSecret, donationRoutingKeySecret, donationEvent);
            _logger.Information($"Donation request sent to StockService. The donation receipt will be sent by email to the donor.");

            return ResultViewModel<Guid>.Success(donation.Id);
        }
    }
}

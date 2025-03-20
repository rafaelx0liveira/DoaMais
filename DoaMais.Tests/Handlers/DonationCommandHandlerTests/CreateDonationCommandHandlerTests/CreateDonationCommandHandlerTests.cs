using DoaMais.Application.Commands.DonationCommands.CreateDonationCommand;
using DoaMais.Application.DTOs;
using DoaMais.Application.Handlers.DonationCommandHandler.CreateDonationCommandHandler;
using DoaMais.Application.Services.DonorService;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.MessageBus.Interface;
using DoaMais.Tests.Utils;
using Microsoft.Extensions.Configuration;
using Moq;
using Serilog;
using VaultService.Interface;

namespace DoaMais.Tests.Handlers.DonationCommandHandlerTests.CreateDonationCommandHandlerTests
{
    public class CreateDonationCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMessageBus> _mockMessageBus;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IVaultClient> _mockVaultClient;
        private readonly Mock<ILogger> _mockLogger;
        private readonly CreateDonationCommandHandler _handler;

        public CreateDonationCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMessageBus = new Mock<IMessageBus>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockVaultClient = new Mock<IVaultClient>();
            _mockLogger = new Mock<ILogger>();

            _mockConfiguration.Setup(x => x["KeyVaultSecrets:RabbitMQ:DonationQueue"]).Returns("queue");
            _mockVaultClient.Setup(x => x.GetSecret("queue")).Returns("queueName");

            _mockConfiguration.Setup(x => x["KeyVaultSecrets:RabbitMQ:StockEventsExchange"]).Returns("exchange");
            _mockVaultClient.Setup(x => x.GetSecret("exchange")).Returns("exchangeName");

            _mockConfiguration.Setup(x => x["KeyVaultSecrets:RabbitMQ:DonationRoutingKey"]).Returns("routingKey");
            _mockVaultClient.Setup(x => x.GetSecret("routingKey")).Returns("routingKeyName");

            _handler = new CreateDonationCommandHandler(
                _mockUnitOfWork.Object,
                _mockMessageBus.Object,
                _mockConfiguration.Object,
                _mockVaultClient.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenDonorDoesNotExist()
        {
            var command = new CreateDonationCommand(
                TestsUtils.CreateMockedGuid(),
                TestsUtils.CreateMockedInt()
            );

            _mockUnitOfWork
                .Setup(x => x.Donors.GetDonorByIdAsync(command.DonorId))
                .ReturnsAsync((Donor)null); 

            var cancellationToken = new CancellationToken();

            var result = await _handler.Handle(command, cancellationToken);

            Assert.False(result.IsSuccess);
            Assert.Contains("was not found", result.Message);
            _mockLogger.Verify(x => x.Warning(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenDonorCannotDonate()
        {
            var command = new CreateDonationCommand(
                TestsUtils.CreateMockedGuid(),
                TestsUtils.CreateMockedInt()
            );

            var donor = new Donor
            {
                Id = command.DonorId,
                DateOfBirth = DateTime.UtcNow.AddYears(-30),
                BiologicalSex = BiologicalSex.Male
            };

            var lastDonation = new Donation { DonationDate = DateTime.UtcNow.AddDays(-30) };

            _mockUnitOfWork.Setup(x => x.Donors.GetDonorByIdAsync(command.DonorId)).ReturnsAsync(donor);
            _mockUnitOfWork.Setup(x => x.Donation.GetLastDonationAsync(command.DonorId)).ReturnsAsync(lastDonation);

            var cancellationToken = new CancellationToken();

            var result = await _handler.Handle(command, cancellationToken);

            Assert.False(result.IsSuccess);
            Assert.Contains("cannot make donation", result.Message);
            _mockLogger.Verify(x => x.Warning(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPublishMessage_WhenDonationIsSuccessful()
        {
            var command = new CreateDonationCommand(
                TestsUtils.CreateMockedGuid(),
                TestsUtils.CreateMockedInt()
            );

            var donor = new Donor
            {
                Id = command.DonorId,
                Name = "Test Donor",
                Email = "test@example.com",
                BloodType = BloodType.O,
                RHFactor = RHFactor.Positive,
                DateOfBirth = DateTime.UtcNow.AddYears(-30),
                BiologicalSex = BiologicalSex.Male
            };

            var lastDonation = new Donation { DonationDate = DateTime.UtcNow.AddDays(-100) };

            var donation = new Donation
            {
                Id = Guid.NewGuid(),
                DonorId = donor.Id,
                Donor = donor, 
                QuantityML = 500
            };

            var donationDTO = new DonationRegisteredEventDTO(
                donation.DonorId,
                donation.Donor.Name,
                donation.Donor.Email,
                donation.Donor.BloodType,
                donation.Donor.RHFactor,
                donation.QuantityML
            );

            _mockUnitOfWork.Setup(x => x.Donors.GetDonorByIdAsync(command.DonorId)).ReturnsAsync(donor);
            _mockUnitOfWork.Setup(x => x.Donation.GetLastDonationAsync(command.DonorId)).ReturnsAsync(lastDonation);

            _mockUnitOfWork.Setup(x => x.Donation.AddDonationAsync(It.IsAny<Donation>()))
                .Callback<Donation>(d => d.Donor = donor) 
                .ReturnsAsync(donation.Id);

            _mockUnitOfWork.Setup(x => x.CompleteAsync()).ReturnsAsync(1);

            _mockMessageBus.Setup(x => x.PublishDirectMessageAsync("exchangeName", "queueName", "routingKeyName", It.IsAny<DonationRegisteredEventDTO>()))
                .Returns(Task.CompletedTask);

            var cancellationToken = new CancellationToken();

            var result = await _handler.Handle(command, cancellationToken);

            Assert.True(result.IsSuccess);
            _mockLogger.Verify(x => x.Information(It.IsAny<string>()), Times.Once);
            _mockMessageBus.Verify(x => x.PublishDirectMessageAsync("exchangeName", "queueName", "routingKeyName", It.IsAny<DonationRegisteredEventDTO>()), Times.Once);
        }

    }


}

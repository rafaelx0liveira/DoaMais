
using DoaMais.Application.Commands.BloodTransfusionCommands.CreateBloodTransfusionCommand;
using DoaMais.Application.DTOs;
using DoaMais.Application.Handlers.BloodTransfusionCommandHandler.CreateBloodTransfusionCommandHandler;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.MessageBus.Interface;
using DoaMais.Tests.Utils;
using Microsoft.Extensions.Configuration;
using Moq;
using Serilog;
using VaultService.Interface;

namespace DoaMais.Tests.Handlers.BloodTransfusionCommandHandlerTests.CreateBloodTransfusionCommandHandlerTests
{
    public class CreateBloodTransfusionCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMessageBus> _mockMessageBus;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IVaultClient> _mockVaultClient;
        private readonly Mock<ILogger> _mockLogger;
        private readonly CreateBloodTransfusionCommandHandler _createBloodTransfusionCommandHandler;

        public CreateBloodTransfusionCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMessageBus = new Mock<IMessageBus>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockVaultClient = new Mock<IVaultClient>();
            _mockLogger = new Mock<ILogger>();

            _mockConfiguration
                .Setup(x => x["KeyVaultSecrets:RabbitMQ:TransfusionQueue"])
                .Returns("TransfusionQueue");

            _mockConfiguration
                .Setup(x => x["KeyVaultSecrets:RabbitMQ:StockEventsExchange"])
                .Returns("StockEventsExchange");

            _mockConfiguration
                .Setup(x => x["KeyVaultSecrets:RabbitMQ:TransfusionRoutingKey"])
                .Returns("TransfusionRoutingKey");

            _mockVaultClient
                .Setup(x => x.GetSecret("TransfusionQueue"))
                .Returns("transfusionQueue");

            _mockVaultClient
                .Setup(x => x.GetSecret("StockEventsExchange"))
                .Returns("stockEventsExchange");

            _mockVaultClient
                .Setup(x => x.GetSecret("TransfusionRoutingKey"))
                .Returns("transfusionRoutingKey");

            _createBloodTransfusionCommandHandler = new CreateBloodTransfusionCommandHandler(
                _mockUnitOfWork.Object,
                _mockMessageBus.Object,
                _mockConfiguration.Object,
                _mockVaultClient.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public void Handle_ShouldReturnError_WhenHospitalIsNull()
        {
            var createBloodTransfusionCommand = new CreateBloodTransfusionCommand(
                TestsUtils.CreateMockedString(),
                TestsUtils.CreateMockedInt(),
                TestsUtils.CreateMockedEnum<BloodType>(),
                TestsUtils.CreateMockedEnum<RHFactor>()
            );

            _mockUnitOfWork
                .Setup(x => x.Hospital.GetHospitalByCNPJAsync(It.IsAny<string>()))
                .ReturnsAsync((Hospital)null);

            var result = _createBloodTransfusionCommandHandler.Handle(createBloodTransfusionCommand, CancellationToken.None).Result;

            Assert.False(result.IsSuccess);
            Assert.Equal($"Hospital with CNPJ {createBloodTransfusionCommand.CNPJ} not found.", result.Message);
        }

        [Fact]
        public void Handle_ShouldReturnError_WhenHospitalIsNotActive()
        {
            var createBloodTransfusionCommand = new CreateBloodTransfusionCommand(
                TestsUtils.CreateMockedString(),
                TestsUtils.CreateMockedInt(),
                TestsUtils.CreateMockedEnum<BloodType>(),
                TestsUtils.CreateMockedEnum<RHFactor>()
            );
            var hospital = new Hospital
            {
                IsActive = false
            };

            _mockUnitOfWork
                .Setup(x => x.Hospital.GetHospitalByCNPJAsync(It.IsAny<string>()))
                .ReturnsAsync(hospital);

            var result = _createBloodTransfusionCommandHandler.Handle(createBloodTransfusionCommand, CancellationToken.None).Result;

            Assert.False(result.IsSuccess);
            Assert.Equal($"Hospital with CNPJ {createBloodTransfusionCommand.CNPJ} not found.", result.Message);
        }

        [Fact]
        public void Handle_ShouldReturnSuccess_WhenHospitalIsValid()
        {
            var createBloodTransfusionCommand = new CreateBloodTransfusionCommand(
                TestsUtils.CreateMockedString(),
                TestsUtils.CreateMockedInt(),
                TestsUtils.CreateMockedEnum<BloodType>(),
                TestsUtils.CreateMockedEnum<RHFactor>()
            );
            var hospital = new Hospital
            {
                IsActive = true
            };

            _mockUnitOfWork
                .Setup(x => x.Hospital.GetHospitalByCNPJAsync(It.IsAny<string>()))
                .ReturnsAsync(hospital);

            var result = _createBloodTransfusionCommandHandler.Handle(createBloodTransfusionCommand, CancellationToken.None).Result;

            Assert.True(result.IsSuccess);
            Assert.Equal("Pedido de transfusão enviado. Aguarde a confirmação do StockService.", result.Message);
        }
    }
}

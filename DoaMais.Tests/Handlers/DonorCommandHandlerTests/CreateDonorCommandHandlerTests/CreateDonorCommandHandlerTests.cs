using DoaMais.Application.Commands.DonorCommands.CreateDonorCommand;
using DoaMais.Application.DTOs;
using DoaMais.Application.Handlers.DonorCommandHandler.CreateDonorCommandHandler;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.Tests.Utils;
using Moq;
using Serilog;

namespace DoaMais.Tests.Handlers.DonorCommandHandlerTests.CreateDonorCommandHandlerTests
{
    public class CreateDonorCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger> _mockIlogger;
        private readonly CreateDonorCommandHandler _createDonorCommandHandler;

        public CreateDonorCommandHandlerTests()
        {
            _mockIlogger = new Mock<ILogger>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _createDonorCommandHandler = new CreateDonorCommandHandler(_mockUnitOfWork.Object, _mockIlogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenDonorExists()
        {
            var address = TestsUtils.CreateMockedObject<AddressDTO>();

            var command = new CreateDonorCommand
            (
                "Test Donor",
                "email@example",
                DateTime.UtcNow,
                TestsUtils.CreateMockedEnum<BiologicalSex>(),
                70,
                TestsUtils.CreateMockedEnum<BloodType>(),
                TestsUtils.CreateMockedEnum<RHFactor>(),
                address
            );

            _mockUnitOfWork.Setup(x => x.Donors.DonorExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            var result = await _createDonorCommandHandler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("Donor with this email already exists", result.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAddressNotExists()
        {
            var address = TestsUtils.CreateMockedObject<AddressDTO>();
            var command = new CreateDonorCommand
            (
                "Test Donor",
                "email@example",
                DateTime.UtcNow,
                TestsUtils.CreateMockedEnum<BiologicalSex>(),
                70,
                TestsUtils.CreateMockedEnum<BloodType>(),
                TestsUtils.CreateMockedEnum<RHFactor>(),
                address
            );

            _mockUnitOfWork.Setup(x => x.Donors.DonorExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            _mockUnitOfWork.Setup(x => x.Address.GetAddressPostalCodeAsync(It.IsAny<string>())).ReturnsAsync((Address)null);

            _mockUnitOfWork.Setup(x => x.Address.AddAddressAsync(It.IsAny<Address>())).Returns(Task.CompletedTask);

            var result = await _createDonorCommandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.Data.ToString());
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAddressExists()
        {
            var address = TestsUtils.CreateMockedObject<AddressDTO>();
            var command = new CreateDonorCommand
            (
                "Test Donor",
                "email@example",
                DateTime.UtcNow,
                TestsUtils.CreateMockedEnum<BiologicalSex>(),
                70,
                TestsUtils.CreateMockedEnum<BloodType>(),
                TestsUtils.CreateMockedEnum<RHFactor>(),
                address
            );

            _mockUnitOfWork.Setup(x => x.Donors.DonorExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mockUnitOfWork.Setup(x => x.Address.GetAddressPostalCodeAsync(It.IsAny<string>())).ReturnsAsync(TestsUtils.CreateMockedObject<Address>());

            var result = await _createDonorCommandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.Data.ToString());
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenAddDonorAsyncThrowsException()
        {
            var address = TestsUtils.CreateMockedObject<AddressDTO>();
            var command = new CreateDonorCommand
            (
                "Test Donor",
                "email@example",
                DateTime.UtcNow,
                TestsUtils.CreateMockedEnum<BiologicalSex>(),
                70,
                TestsUtils.CreateMockedEnum<BloodType>(),
                TestsUtils.CreateMockedEnum<RHFactor>(),
                address
            );

            _mockUnitOfWork.Setup(x => x.Donors.DonorExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mockUnitOfWork.Setup(x => x.Address.GetAddressPostalCodeAsync(It.IsAny<string>())).ReturnsAsync(TestsUtils.CreateMockedObject<Address>());
            _mockUnitOfWork.Setup(x => x.Donors.AddDonorAsync(It.IsAny<Donor>())).Throws(new Exception());

            var result = await _createDonorCommandHandler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Message);
        }
    }
}

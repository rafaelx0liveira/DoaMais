using DoaMais.Application.Commands.HospitalCommands.CreateHospitalCommand;
using DoaMais.Application.Handlers.HospitalCommandHandler.CreateHospitalCommandHandler;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using Moq;
using Serilog;

namespace DoaMais.Tests.Handlers.HospitalCommandHandlerTests.CreateHospitalCommandHandlerTests
{
    public class CreateHospitalCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger> _mockIlogger;
        private readonly CreateHospitalCommandHandler _createHospitalCommandHandler;
        public CreateHospitalCommandHandlerTests()
        {
            _mockIlogger = new Mock<ILogger>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _createHospitalCommandHandler = new CreateHospitalCommandHandler(_mockUnitOfWork.Object, _mockIlogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenHospitalExists()
        {
            var command = new CreateHospitalCommand
            (
                "Test Hospital",
                "cnpj",
                "email@example",
                "phone"
            );
            _mockUnitOfWork.Setup(x => x.Hospital.HospitalExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            var result = await _createHospitalCommandHandler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("Hospital with CNPJ " + command.CNPJ + " already exists", result.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenHospitalNotExists()
        {
            var command = new CreateHospitalCommand
            (
                "Test Hospital",
                "cnpj",
                "email@example",
                "phone"
            );
            _mockUnitOfWork.Setup(x => x.Hospital.HospitalExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            var result = await _createHospitalCommandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenExceptionOccurs()
        {
            var command = new CreateHospitalCommand
            (
                "Test Hospital",
                "cnpj",
                "email@example",
                "phone"
            );

            _mockUnitOfWork.Setup(x => x.Hospital.HospitalExistsAsync(It.IsAny<string>())).Throws(new Exception());

            var result = await _createHospitalCommandHandler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
        }
    }
}

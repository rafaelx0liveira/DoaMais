using DoaMais.Application.Commands.EmployeeCommands.CreateEmployeeCommand;
using DoaMais.Application.DTOs;
using DoaMais.Application.Handlers.EmployeeCommandHandler.CreateEmployeeCommandHandler;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.Tests.Utils;
using Moq;
using Serilog;

namespace DoaMais.Tests.Handlers.EmployeeCommandHandlerTests.CreateEmployeeCommandHandlerTests
{
    public class CreateEmployeeCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger> _mockIlogger;
        private readonly CreateEmployeeCommandHandler _createEmployeeCommandHandler;

        public CreateEmployeeCommandHandlerTests()
        {
            _mockIlogger = new Mock<ILogger>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _createEmployeeCommandHandler = new CreateEmployeeCommandHandler(_mockUnitOfWork.Object, _mockIlogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenEmployeeExists()
        {
            var command = new CreateEmployeeCommand
            (
                "Test Employee",
                "email@example",
                "password",
                TestsUtils.CreateMockedEnum<EmployeeRole>(),
                TestsUtils.CreateMockedObject<AddressDTO>()
            );

            _mockUnitOfWork.Setup(x => x.Employee.EmployeeExists(It.IsAny<string>())).ReturnsAsync(true);

            var result = await _createEmployeeCommandHandler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("Employee with email " + command.Email + " already exists", result.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAddressNotExists()
        {
            var command = new CreateEmployeeCommand
            (
                "Test Employee",
                "email@example",
                "password",
                TestsUtils.CreateMockedEnum<EmployeeRole>(),
                TestsUtils.CreateMockedObject<AddressDTO>()
            );

            _mockUnitOfWork.Setup(x => x.Employee.EmployeeExists(It.IsAny<string>())).ReturnsAsync(false);
            _mockUnitOfWork.Setup(x => x.Address.GetAddressPostalCodeAsync(It.IsAny<string>())).ReturnsAsync((Address)null);

            var result = await _createEmployeeCommandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAddressExists()
        {
            var command = new CreateEmployeeCommand
            (
                "Test Employee",
                "email@example",
                "password",
                TestsUtils.CreateMockedEnum<EmployeeRole>(),
                TestsUtils.CreateMockedObject<AddressDTO>()
            );

            _mockUnitOfWork.Setup(x => x.Employee.EmployeeExists(It.IsAny<string>())).ReturnsAsync(false);
            _mockUnitOfWork.Setup(x => x.Address.GetAddressPostalCodeAsync(It.IsAny<string>())).ReturnsAsync(new Address());

            var result = await _createEmployeeCommandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenAddEmployeeAsyncThrowsException()
        {
            var command = new CreateEmployeeCommand
            (
                "Test Employee",
                "email@example",
                "password",
                TestsUtils.CreateMockedEnum<EmployeeRole>(),
                TestsUtils.CreateMockedObject<AddressDTO>()
            );

            _mockUnitOfWork.Setup(x => x.Employee.EmployeeExists(It.IsAny<string>())).ReturnsAsync(false);
            _mockUnitOfWork.Setup(x => x.Address.GetAddressPostalCodeAsync(It.IsAny<string>())).ReturnsAsync((Address)null);
            _mockUnitOfWork.Setup(x => x.Employee.AddEmployeeAsync(It.IsAny<Employee>())).ThrowsAsync(new Exception());

            var result = await _createEmployeeCommandHandler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("One or more errors occurred: Exception of type 'System.Exception' was thrown.", result.Message);
        }
    }
}

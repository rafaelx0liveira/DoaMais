using DoaMais.Application.Services.Interface;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using Moq;
using Serilog;
using DoaMais.Application.Handlers.AuthCommandHandler.LoginCommandHandler;
using DoaMais.Domain.Entities;
using DoaMais.Application.Commands.AuthCommands.LoginCommand;
using DoaMais.Tests.Utils;

namespace DoaMais.Tests.Handlers.AuthCommandHandlerTests.LoginCommandHandlerTests
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger> _mockLogger;
        private readonly LoginCommandHandler _loginCommandHandler;

        public LoginCommandHandlerTests()
        {
            _mockTokenService = new Mock<ITokenService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger>();
            _loginCommandHandler = new LoginCommandHandler(_mockTokenService.Object, _mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public void Handle_ShouldReturnError_WhenUserIsNull()
        {
            var login = new LoginCommand(TestsUtils.CreateMockedEmail(), TestsUtils.CreateMockedString());

            _mockUnitOfWork
                .Setup(x => x.Employee.GetEmployeeByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Employee)null);

            var result = _loginCommandHandler.Handle(login, CancellationToken.None).Result;

            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid email or password", result.Message);
        }

        [Fact]
        public void Handle_ShouldReturnSuccess_WhenUserIsValid()
        {
            var login = new LoginCommand(TestsUtils.CreateMockedEmail(), TestsUtils.CreateMockedString());
            var user = new Employee
            {
                Email = TestsUtils.CreateMockedEmail(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(TestsUtils.CreateMockedString())
            };

            _mockUnitOfWork
                .Setup(x => x.Employee.GetEmployeeByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _mockTokenService
                .Setup(x => x.GenerateToken(It.IsAny<Employee>()))
                .Returns(TestsUtils.CreateMockedString());

            var result = _loginCommandHandler.Handle(login, CancellationToken.None).Result;

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }
    }
}

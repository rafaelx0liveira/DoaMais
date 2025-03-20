
using DoaMais.Application.Handlers.EmployeeQueryHandler.GetAllEmployeeQueryHandler;
using DoaMais.Application.Queries.EmployeesQueries.GetAllEmployeesQuery;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.Tests.Utils;
using Moq;

namespace DoaMais.Tests.Handlers.EmployeeQueryHandlerTests.GetAllEmployeeQueryHandlerTests
{
    public class GetAllEmployeeQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetAllEmployeesQueryHandler _getAllEmployeeQueryHandler;
        public GetAllEmployeeQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _getAllEmployeeQueryHandler = new GetAllEmployeesQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmployees()
        {
            var adress = TestsUtils.CreateMockedObject<Address>();

            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = TestsUtils.CreateMockedGuid(),
                    Name = "Test Employee",
                    Email = "email@example",
                    Role = EmployeeRole.Admin,
                    Address = adress
                }
            };
            _mockUnitOfWork.Setup(x => x.Employee.GetAllEmployeesAsync()).ReturnsAsync(employees);
            var result = await _getAllEmployeeQueryHandler.Handle(new GetAllEmployeesQuery(), CancellationToken.None);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList()
        {
            _mockUnitOfWork.Setup(x => x.Employee.GetAllEmployeesAsync()).ReturnsAsync(new List<Employee>());
            var result = await _getAllEmployeeQueryHandler.Handle(new GetAllEmployeesQuery(), CancellationToken.None);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data);
        }
    }
}

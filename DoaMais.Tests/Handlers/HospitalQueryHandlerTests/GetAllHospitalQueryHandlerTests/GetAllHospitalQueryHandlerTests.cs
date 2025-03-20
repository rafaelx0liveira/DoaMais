
using DoaMais.Application.Handlers.HospitalQueryHandler.GetAllHospitalsQueryHandler;
using DoaMais.Application.Queries.HospitalQueries.GetAllHospitalsQuery;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.Tests.Utils;
using Moq;

namespace DoaMais.Tests.Handlers.HospitalQueryHandlerTests.GetAllHospitalQueryHandlerTests
{
    public class GetAllHospitalQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetAllHospitalsQueryHandler _getAllHospitalQueryHandler;
        public GetAllHospitalQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _getAllHospitalQueryHandler = new GetAllHospitalsQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnHospitals()
        {
            var adress = TestsUtils.CreateMockedObject<Address>();
            var hospitals = new List<Hospital>
            {
                new Hospital
                {
                    Id = TestsUtils.CreateMockedGuid(),
                    Name = "Test Hospital",
                    CNPJ = "cnpj",
                    Email = "email@example",
                    Phone = "phone"
                }
            };

            _mockUnitOfWork.Setup(x => x.Hospital.GetAllHospitalAsync()).ReturnsAsync(hospitals);

            var result = await _getAllHospitalQueryHandler.Handle(new GetAllHospitalsQuery(), CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList()
        {
            _mockUnitOfWork.Setup(x => x.Hospital.GetAllHospitalAsync()).ReturnsAsync(new List<Hospital>());

            var result = await _getAllHospitalQueryHandler.Handle(new GetAllHospitalsQuery(), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data);
        }
    }
}

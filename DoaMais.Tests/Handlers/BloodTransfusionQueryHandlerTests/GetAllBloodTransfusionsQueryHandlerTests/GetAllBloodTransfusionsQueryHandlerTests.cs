using DoaMais.Application.Handlers.BloodTransfusionQueryHandler.GetAllBloodTransfusionsQueryHandler;
using DoaMais.Application.Queries.BloodTransfusionsQueries.GetAllBloodTransfusionsQuery;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.Tests.Utils;
using Moq;

namespace DoaMais.Tests.Handlers.BloodTransfusionQueryHandlerTests.GetAllBloodTransfusionsQueryHandlerTests
{
    public class GetAllBloodTransfusionsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetAllBloodTransfusionsQueryHandler _getAllBloodTransfusionsQueryHandler;

        public GetAllBloodTransfusionsQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _getAllBloodTransfusionsQueryHandler = new GetAllBloodTransfusionsQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public void Handle_ShouldReturnBloodTransfusions()
        {
            var bloodTransfusions = new List<BloodTransfusion>
            {
                new BloodTransfusion
                {
                    Id = Guid.NewGuid(),
                    HospitalId = Guid.NewGuid(),
                    Hospital = new Hospital
                    {
                        Id = Guid.NewGuid(),
                        Name = "Hospital Central",
                        CNPJ = "00.000.000/0001-00",
                        Phone = "1111-2222",
                        Email = "hospital@example.com"
                    },
                    QuantityML = 500,
                    BloodType = BloodType.O,
                    RHFactor = RHFactor.Positive,
                    TransfusionDate = DateTime.UtcNow
                }
            };

            _mockUnitOfWork
                .Setup(x => x.BloodTransfusion.GetAllBloodTransfusionsAsync())
                .ReturnsAsync(bloodTransfusions);

            var result = _getAllBloodTransfusionsQueryHandler.Handle(new GetAllBloodTransfusionsQuery(), CancellationToken.None).Result;

            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.Data);
        }
    }
}

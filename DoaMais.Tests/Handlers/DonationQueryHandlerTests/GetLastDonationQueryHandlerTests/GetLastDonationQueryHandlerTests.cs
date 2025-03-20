using DoaMais.Application.Handlers.DonationQueryHandler.GetLastDonationQueryHandler;
using DoaMais.Application.Queries.DonationQueries.GetLastDonationQuery;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.Domain.Interfaces.Repository.DonationRepository;
using DoaMais.Tests.Utils;
using Moq;

namespace DoaMais.Tests.Handlers.DonationQueryHandlerTests.GetLastDonationQueryHandlerTests
{
    public class GetLastDonationQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetLastDonationQueryHandler _getLastDonationQueryHandler;
        public GetLastDonationQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _getLastDonationQueryHandler = new GetLastDonationQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenDonorHasNoDonations()
        {
            var query = new GetLastDonationQuery(TestsUtils.CreateMockedGuid());

            _mockUnitOfWork.Setup(x => x.Donation.GetLastDonationAsync(It.IsAny<Guid>())).ReturnsAsync((Donation)null);

            var result = await _getLastDonationQueryHandler.Handle(query, CancellationToken.None);
            
            Assert.False(result.IsSuccess);
            Assert.Equal("Donor with " + query.DonorId + " has no last donations", result.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnDonation()
        {
            var lastDonation = new Donation
            {
                Id = TestsUtils.CreateMockedGuid(),
                DonationDate = DateTime.UtcNow.AddDays(-30),
                QuantityML = 500,
                Donor = new Donor
                {
                    Id = TestsUtils.CreateMockedGuid(),
                    Name = "Test Donor",
                    Email = "email@example",
                    Address = new Address
                    {
                        StreetAddress = "address",
                        City = "city",
                        State = "state",
                        PostalCode = "00000-000"
                    }
                }
            };

            _mockUnitOfWork.Setup(x => x.Donation.GetLastDonationAsync(It.IsAny<Guid>())).ReturnsAsync(lastDonation);

            var result = _getLastDonationQueryHandler.Handle(new GetLastDonationQuery(TestsUtils.CreateMockedGuid()), CancellationToken.None).Result;

            Assert.True(result.IsSuccess);
            Assert.Equal(lastDonation.Id, result?.Data?.DonationId);
        }
    }
}

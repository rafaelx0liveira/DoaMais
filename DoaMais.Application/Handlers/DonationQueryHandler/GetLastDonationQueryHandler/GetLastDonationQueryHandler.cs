using DoaMais.Application.Models;
using DoaMais.Application.Queries.DonationQueries.GetLastDonationQuery;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.DonationQueryHandler.GetLastDonationQueryHandler
{
    public class GetLastDonationQueryHandler (IUnitOfWork unitOfWork) : IRequestHandler<GetLastDonationQuery, ResultViewModel<DonationViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultViewModel<DonationViewModel>> Handle(GetLastDonationQuery request, CancellationToken cancellationToken)
        {
            var lastDonation = await _unitOfWork.Donation.GetLastDonationAsync(request.DonorId);

            if (lastDonation == null)
                return ResultViewModel<DonationViewModel>.Error($"Donor with {request.DonorId} has no last donations");

            var donationViewModel = DonationViewModel.FromEntity(lastDonation);
            return ResultViewModel<DonationViewModel>.Success(donationViewModel);
        }
    }
}

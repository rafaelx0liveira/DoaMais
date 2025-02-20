using DoaMais.Application.Models;
using DoaMais.Application.Queries.DonorsQueries.GetDonorByIdQuery;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.DonorQueryHandler.GetDonorByIdQueryHandler
{
    public class GetDonorByIdQueryHandler (IUnitOfWork unitOfWork) : IRequestHandler<GetDonorByIdQuery, ResultViewModel<DonorViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultViewModel<DonorViewModel>> Handle(GetDonorByIdQuery request, CancellationToken cancellationToken)
        {
            var donor = await _unitOfWork.Donors.GetDonorByIdAsync(request.Id);

            if (donor == null) 
                return ResultViewModel<DonorViewModel>.Error($"Donor with Id {request.Id} was not found.");

            var donorViewModel = DonorViewModel.FromEntity(donor);

            return ResultViewModel<DonorViewModel>.Success(donorViewModel);
        }
    }
}

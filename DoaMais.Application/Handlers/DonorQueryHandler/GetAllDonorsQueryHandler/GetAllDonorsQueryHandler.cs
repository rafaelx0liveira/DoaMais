using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Application.Queries.DonorsQueries.GetAllDonorsQuery;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.DonorQueryHandler.GetAllDonorsQueryHandler
{
    public class GetAllDonorsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllDonorsQuery, ResultViewModel<IEnumerable<DonorViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultViewModel<IEnumerable<DonorViewModel>>> Handle(GetAllDonorsQuery request, CancellationToken cancellationToken)
        {
            var donors = await _unitOfWork.Donors.GetAllDonorsAsync();

            var result = donors.Select(DonorViewModel.FromEntity).ToList();

            return ResultViewModel<IEnumerable<DonorViewModel>>.Success(result);
        }
    }
}

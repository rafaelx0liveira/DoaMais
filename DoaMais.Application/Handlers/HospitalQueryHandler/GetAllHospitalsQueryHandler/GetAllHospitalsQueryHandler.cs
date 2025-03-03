using DoaMais.Application.Models;
using DoaMais.Application.Queries.HospitalQueries.GetAllHospitalsQuery;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.HospitalQueryHandler.GetAllHospitalsQueryHandler
{
    public class GetAllHospitalsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllHospitalsQuery, ResultViewModel<IEnumerable<HospitalViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultViewModel<IEnumerable<HospitalViewModel>>> Handle(GetAllHospitalsQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Hospital.GetAllHospitalAsync();

            var hospitals = result.Select(HospitalViewModel.FromEntity).ToList();

            return ResultViewModel<IEnumerable<HospitalViewModel>>.Success(hospitals);
        }
    }
}

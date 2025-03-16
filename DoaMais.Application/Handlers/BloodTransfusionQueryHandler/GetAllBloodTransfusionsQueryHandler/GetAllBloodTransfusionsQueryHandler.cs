using DoaMais.Application.Models;
using DoaMais.Application.Queries.BloodTransfusionsQueries.GetAllBloodTransfusionsQuery;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.BloodTransfusionQueryHandler.GetAllBloodTransfusionsQueryHandler
{
    class GetAllBloodTransfusionsQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetAllBloodTransfusionsQuery, ResultViewModel<IEnumerable<BloodTransfusionViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultViewModel<IEnumerable<BloodTransfusionViewModel>>> Handle(GetAllBloodTransfusionsQuery request, CancellationToken cancellationToken)
        {
            var bloodTransfusions = await _unitOfWork.BloodTransfusion.GetAllBloodTransfusionsAsync();

            var result = bloodTransfusions.Select(BloodTransfusionViewModel.FromEntity).ToList();

            return ResultViewModel<IEnumerable<BloodTransfusionViewModel>>.Success(result);
        }
    }
}

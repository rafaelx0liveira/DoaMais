using DoaMais.Application.Models;
using DoaMais.Application.Queries.ReportQueries.GetDonationReportQuery;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.ReportQueryHandler.GetDonationReportQueryHandler
{
    public class GetDonationReportQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDonationReportQuery, ResultViewModel<ReportViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultViewModel<ReportViewModel>> Handle(GetDonationReportQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Report.GetLastDonationsReportAsync();

            if(result == null)
            {
                return ResultViewModel<ReportViewModel>.Error("Not found blood donations report");
            }

            var report = ReportViewModel.FromEntity(result.FileData);

            return ResultViewModel<ReportViewModel>.Success(report);
        }
    }
}

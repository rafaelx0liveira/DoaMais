
using DoaMais.Application.Models;
using DoaMais.Application.Queries.ReportQueries.GetBloodStockReportQuery;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.ReportQueryHandler.GetBloodStockReportQueryHandler
{
    public class GetBloodStockReportQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetBloodStockReportQuery, ResultViewModel<ReportViewModel>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultViewModel<ReportViewModel>> Handle(GetBloodStockReportQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Report.GetLastBloodStockReportAsync();

            if (result == null)
            {
                return ResultViewModel<ReportViewModel>.Error("Not found blood stock report");
            }

            var report = ReportViewModel.FromEntity(result.FileData);

            return ResultViewModel<ReportViewModel>.Success(report);
        }
    }
}

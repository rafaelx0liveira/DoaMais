using DoaMais.Application.Models;
using MediatR;

namespace DoaMais.Application.Queries.ReportQueries.GetDonationReportQuery
{
    public class GetDonationReportQuery : IRequest<ResultViewModel<ReportViewModel>>
    {
    }
}

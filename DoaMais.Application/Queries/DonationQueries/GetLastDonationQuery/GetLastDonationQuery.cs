using DoaMais.Application.Models;
using MediatR;

namespace DoaMais.Application.Queries.DonationQueries.GetLastDonationQuery
{
    public class GetLastDonationQuery : IRequest<ResultViewModel<DonationViewModel>>
    {
        public Guid DonorId { get; set; }

        public GetLastDonationQuery(Guid donorId)
        {
            DonorId = donorId;
        }
    }
}

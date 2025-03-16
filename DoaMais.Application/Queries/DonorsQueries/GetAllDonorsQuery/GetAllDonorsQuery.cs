using DoaMais.Application.Models;
using MediatR;

namespace DoaMais.Application.Queries.DonorsQueries.GetAllDonorsQuery
{
    public class GetAllDonorsQuery : IRequest<ResultViewModel<IEnumerable<DonorViewModel>>>
    {
    }
}

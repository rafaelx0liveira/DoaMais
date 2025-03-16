using DoaMais.Application.Models;
using MediatR;

namespace DoaMais.Application.Queries.BloodTransfusionsQueries.GetAllBloodTransfusionsQuery
{
    public class GetAllBloodTransfusionsQuery : IRequest<ResultViewModel<IEnumerable<BloodTransfusionViewModel>>>
    {
    }
}

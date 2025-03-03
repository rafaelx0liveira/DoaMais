using DoaMais.Application.Models;
using MediatR;

namespace DoaMais.Application.Queries.HospitalQueries.GetAllHospitalsQuery
{
    public class GetAllHospitalsQuery : IRequest<ResultViewModel<IEnumerable<HospitalViewModel>>>
    {
    }
}

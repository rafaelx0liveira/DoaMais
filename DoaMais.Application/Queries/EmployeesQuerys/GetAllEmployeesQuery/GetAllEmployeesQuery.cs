using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using MediatR;

namespace DoaMais.Application.Queries.EmployeesQuerys.GetAllEmployeesQuery
{
    public class GetAllEmployeesQuery : IRequest<ResultViewModel<IEnumerable<EmployeeViewModel>>>
    {
    }
}

using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Domain.Interfaces.UnityOfWork;
using MediatR;

namespace DoaMais.Application.QueriesAndhandlers.GetAllEmployeesQuery
{
    public class GetAllEmployeesQuery : IRequest<ResultViewModel<IEnumerable<EmployeeDTO>>>
    {
    }
}

using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Application.Queries.EmployeesQueries.GetAllEmployeesQuery;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.EmployeeQueryHandler.GetAllEmployeeQueryHandler
{
    public class GetAllEmployeesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllEmployeesQuery, ResultViewModel<IEnumerable<EmployeeViewModel>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultViewModel<IEnumerable<EmployeeViewModel>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Employee.GetAllEmployeesAsync();

            var employees = result.Select(EmployeeViewModel.FromEntity).ToList();

            return ResultViewModel<IEnumerable<EmployeeViewModel>>.Success(employees);
        }
    }
}

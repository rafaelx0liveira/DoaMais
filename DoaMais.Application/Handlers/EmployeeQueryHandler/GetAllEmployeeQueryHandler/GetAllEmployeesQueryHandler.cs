using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Application.Queries.EmployeesQuerys.GetAllEmployeesQuery;
using DoaMais.Domain.Interfaces.UnityOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.EmployeeQueryHandler.GetAllEmployeeQueryHandler
{
    public class GetAllEmployeesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllEmployeesQuery, ResultViewModel<IEnumerable<EmployeeDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultViewModel<IEnumerable<EmployeeDTO>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _unitOfWork.Employee.GetAllAsync();

            var employeeDTOs = employees.Select(x => new EmployeeDTO
            {
                Name = x.Name,
                Email = x.Email,
                Role = x.Role,
                Address = new AddressDTO
                {
                    StreetAddress = x.Address.StreetAddress,
                    City = x.Address.City,
                    State = x.Address.State,
                    PostalCode = x.Address.PostalCode
                }
            }).ToList();

            return ResultViewModel<IEnumerable<EmployeeDTO>>.Success(employeeDTOs);
        }
    }
}

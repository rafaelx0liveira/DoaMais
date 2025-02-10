using DoaMais.Application.Commands.EmployeeCommands.CreateEmployeeCommand;
using DoaMais.Application.Models;
using DoaMais.Domain.Interfaces.UnityOfWork;
using MediatR;

namespace DoaMais.Application.Handlers.EmployeeCommandHandler.CreateEmployeeCommandHandler
{
    public class CreateEmployeeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateEmployeeCommand, ResultViewModel<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResultViewModel<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employeeExists = await _unitOfWork.Employee.EmployeeExists(request.EmployeeDTO.Email);

                if (employeeExists) return ResultViewModel<Guid>.Error($"Employee with email {request.EmployeeDTO.Email} already exists");

                var employee = request.ToEntity();
                employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.EmployeeDTO.Password);

                await _unitOfWork.Employee.AddAsync(employee);

                return ResultViewModel<Guid>.Success(employee.Id);
            }
            catch (Exception ex)
            {
                return ResultViewModel<Guid>.Error($"One or more errors occurred:{ex.Message}");
            }
        }
    }
}

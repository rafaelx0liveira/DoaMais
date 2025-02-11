using DoaMais.Application.Commands.EmployeeCommands.CreateEmployeeCommand;
using DoaMais.Application.Models;
using DoaMais.Domain.Entities;
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

                if (employeeExists)
                    return ResultViewModel<Guid>.Error($"Employee with email {request.EmployeeDTO.Email} already exists");

                var existingAddress = await _unitOfWork.Address.GetAddressPostalCodeAsync(request.EmployeeDTO.Address.PostalCode);

                Address address;
                if (existingAddress != null)
                {
                    address = existingAddress;
                }
                else
                {
                    address = new Address
                    {
                        StreetAddress = request.EmployeeDTO.Address.StreetAddress,
                        City = request.EmployeeDTO.Address.City,
                        State = request.EmployeeDTO.Address.State,
                        PostalCode = request.EmployeeDTO.Address.PostalCode
                    };

                    await _unitOfWork.Address.AddAddressAsync(address);
                }

                var employee = new Employee
                {
                    Name = request.EmployeeDTO.Name,
                    Email = request.EmployeeDTO.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.EmployeeDTO.Password),
                    Role = request.EmployeeDTO.Role,
                    AddressId = address.Id, 
                    Address = null 
                };

                await _unitOfWork.Employee.AddAsync(employee);

                return ResultViewModel<Guid>.Success(employee.Id);
            }
            catch (Exception ex)
            {
                return ResultViewModel<Guid>.Error($"One or more errors occurred: {ex.Message}");
            }
        }

    }
}

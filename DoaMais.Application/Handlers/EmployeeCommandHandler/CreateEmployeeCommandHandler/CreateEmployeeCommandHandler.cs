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
                var employeeExists = await _unitOfWork.Employee.EmployeeExists(request.Email);

                if (employeeExists)
                    return ResultViewModel<Guid>.Error($"Employee with email {request.Email} already exists");

                var existingAddress = await _unitOfWork.Address.GetAddressPostalCodeAsync(request.Address.PostalCode);

                Address address;
                if (existingAddress != null)
                {
                    address = existingAddress;
                }
                else
                {
                    address = new Address
                    {
                        StreetAddress = request.Address.StreetAddress,
                        City = request.Address.City,
                        State = request.Address.State,
                        PostalCode = request.Address.PostalCode
                    };

                    await _unitOfWork.Address.AddAddressAsync(address);
                }

                var employee = new Employee
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = request.Role,
                    AddressId = address.Id, 
                    Address = null 
                };

                await _unitOfWork.Employee.AddEmployeeAsync(employee);

                return ResultViewModel<Guid>.Success(employee.Id);
            }
            catch (Exception ex)
            {
                return ResultViewModel<Guid>.Error($"One or more errors occurred: {ex.Message}");
            }
        }

    }
}

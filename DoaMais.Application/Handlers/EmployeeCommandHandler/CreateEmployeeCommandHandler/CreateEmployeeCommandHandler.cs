using DoaMais.Application.Commands.EmployeeCommands.CreateEmployeeCommand;
using DoaMais.Application.Models;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using MediatR;
using Serilog;

namespace DoaMais.Application.Handlers.EmployeeCommandHandler.CreateEmployeeCommandHandler
{
    public class CreateEmployeeCommandHandler(IUnitOfWork unitOfWork, ILogger logger) : IRequestHandler<CreateEmployeeCommand, ResultViewModel<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger _logger = logger;

        public async Task<ResultViewModel<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employeeExists = await _unitOfWork.Employee.EmployeeExists(request.Email);

                if (employeeExists)
                {
                    _logger.Warning($"Employee with email {request.Email} already exists");
                    return ResultViewModel<Guid>.Error($"Employee with email {request.Email} already exists");
                }

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
                await _unitOfWork.CompleteAsync();

                _logger.Information($"Employee {employee.Id} created successfully");
                return ResultViewModel<Guid>.Success(employee.Id);
            }
            catch (Exception ex)
            {
                _logger.Warning($"One or more errors occurred while creating the employee: {ex.Message}");
                return ResultViewModel<Guid>.Error($"One or more errors occurred: {ex.Message}");
            }
        }

    }
}

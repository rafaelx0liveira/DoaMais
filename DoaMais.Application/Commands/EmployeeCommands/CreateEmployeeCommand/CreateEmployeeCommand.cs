using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Domain.Entities;
using MediatR;

namespace DoaMais.Application.Commands.EmployeeCommands.CreateEmployeeCommand
{
    public class CreateEmployeeCommand : IRequest<ResultViewModel<Guid>>
    {
        public EmployeeDTO EmployeeDTO { get; }

        public CreateEmployeeCommand(EmployeeDTO employeeDTO)
        {
            EmployeeDTO = employeeDTO;
        }

        public Employee ToEntity()
        {
            var address = new Address
            {
                StreetAddress = EmployeeDTO.Address.StreetAddress,
                City = EmployeeDTO.Address.City,
                State = EmployeeDTO.Address.State,
                PostalCode = EmployeeDTO.Address.PostalCode
            };

            return new Employee
            {
                Name = EmployeeDTO.Name,
                Email = EmployeeDTO.Email,
                Role = EmployeeDTO.Role,
                Address = address,
            };
        }
    }
}

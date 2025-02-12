using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Application.Validators;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace DoaMais.Application.Commands.EmployeeCommands.CreateEmployeeCommand
{
    public class CreateEmployeeCommand : IRequest<ResultViewModel<Guid>>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [JsonConverter(typeof(InvalidEnumConverter<EmployeeRole>))]
        public EmployeeRole Role { get; set; }
        public AddressDTO Address { get; set; }

        public CreateEmployeeCommand(string name, string email, string password, EmployeeRole role, AddressDTO address)
        {
            Name = name;
            Email = email;
            Password = password;
            Role = role;
            Address = address;
        }

        public Employee ToEntity()
        {
            var address = new Address
            {
                StreetAddress = Address.StreetAddress,
                City = Address.City,
                State = Address.State,
                PostalCode = Address.PostalCode
            };

            return new Employee
            {
                Name = Name,
                Email = Email,
                Role = Role,
                Address = address,
            };
        }
    }
}

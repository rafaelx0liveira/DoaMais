using DoaMais.Application.DTOs;
using DoaMais.Domain.Entities;
using DoaMais.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DoaMais.Application.Models
{
    public class EmployeeViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public EmployeeRole Role { get; set; }

        public AddressDTO Address { get; set; }

        public EmployeeViewModel(string name, string email, EmployeeRole role, AddressDTO addressDTO)
        {
            Name = name;
            Email = email;
            Role = role;
            Address = addressDTO;
        }

        public static EmployeeViewModel FromEntity(Employee employee)
        {
            return new EmployeeViewModel(
                employee.Name,
                employee.Email,
                employee.Role,
                new AddressDTO
                {
                    StreetAddress = employee.Address.StreetAddress,
                    City = employee.Address.City,
                    State = employee.Address.State,
                    PostalCode = employee.Address.PostalCode
                }   
            );
        }
    }
}

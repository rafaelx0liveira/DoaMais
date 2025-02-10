using DoaMais.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DoaMais.Application.DTOs
{
    public class EmployeeDTO
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("role")]
        public EmployeeRole Role { get; set; }

        [JsonPropertyName("address")]
        public AddressDTO Address { get; set; }
    }

}

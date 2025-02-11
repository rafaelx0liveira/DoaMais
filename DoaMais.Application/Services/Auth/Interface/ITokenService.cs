using DoaMais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.Application.Services.Auth.Interface
{
    public interface ITokenService
    {
        string GenerateToken(Employee user);
    }
}

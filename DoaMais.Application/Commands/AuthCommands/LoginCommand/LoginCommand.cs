using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoaMais.Application.Commands.AuthCommands.LoginCommand
{
    public class LoginCommand : IRequest<ResultViewModel<string>>
    {
        public LoginDTO LoginDTO { get; }

        public LoginCommand(LoginDTO loginDTO)
        {
            LoginDTO = loginDTO;
        }

    }
}

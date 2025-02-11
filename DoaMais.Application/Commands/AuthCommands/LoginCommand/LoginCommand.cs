using DoaMais.Application.Models;
using MediatR;

namespace DoaMais.Application.Commands.AuthCommands.LoginCommand
{
    public class LoginCommand : IRequest<ResultViewModel<string>>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }

    }
}

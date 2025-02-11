using DoaMais.Application.Commands.AuthCommands.LoginCommand;
using DoaMais.Application.Models;
using DoaMais.Domain.Interfaces.UnityOfWork;
using MediatR;
using DoaMais.Application.Services.Auth.Interface;


namespace DoaMais.Application.Handlers.AuthCommandHandler.LoginCommandHandler
{
    public class LoginCommandHandler (ITokenService tokenService, IUnitOfWork unitOfWork) : IRequestHandler<LoginCommand, ResultViewModel<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<ResultViewModel<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Employee.GetEmployeeByEmailAsync(request.Email);

            if(user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return ResultViewModel<string>.Error("Invalid email or password");
            }

            var token = _tokenService.GenerateToken(user);
            return ResultViewModel<string>.Success(token);
        }
    }
}

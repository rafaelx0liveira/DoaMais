using DoaMais.Application.Commands.AuthCommands.LoginCommand;
using DoaMais.Application.Models;
using MediatR;
using DoaMais.Application.Services.Interface;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using Serilog;


namespace DoaMais.Application.Handlers.AuthCommandHandler.LoginCommandHandler
{
    public class LoginCommandHandler (
        ITokenService tokenService, 
        IUnitOfWork unitOfWork,
        ILogger logger) : IRequestHandler<LoginCommand, ResultViewModel<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger _logger = logger;

        public async Task<ResultViewModel<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Employee.GetEmployeeByEmailAsync(request.Email);

            if(user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.Warning($"Invalid email or password: {request.Email}");
                return ResultViewModel<string>.Error("Invalid email or password");
            }

            var token = _tokenService.GenerateToken(user);
            return ResultViewModel<string>.Success(token);
        }
    }
}

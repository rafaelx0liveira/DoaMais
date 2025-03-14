using DoaMais.Application.Commands.AuthCommands.LoginCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DoaMais.API.Controllers
{
    /// <summary>
    /// Auth Controller
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IMediator mediator, Serilog.ILogger logger)
        : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly Serilog.ILogger _logger = logger;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        {
            _logger.Information("Tentativa de login para o e-mail: {Email}", request.Email);

            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                _logger.Warning("Falha no login para o e-mail: {Email}. Motivo: {Motivo}", request.Email, result.Message ?? "Erro desconhecido");
                return Unauthorized(result);
            }

            _logger.Information("Login bem-sucedido para o e-mail: {Email}", request.Email);
            return Ok(result);
        }
    }

}

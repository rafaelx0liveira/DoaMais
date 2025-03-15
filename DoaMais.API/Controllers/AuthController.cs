using DoaMais.Application.Commands.AuthCommands.LoginCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

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
        private readonly ILogger _logger = logger;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        {
            _logger.Information($"Failed login attempt for email: {request.Email}");

            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                _logger.Warning($"Failed login attempt for email: {request.Email}. Reason: {result.Message}");
                return Unauthorized(result);
            }

            _logger.Information($"Successful login for email: {request.Email}");
            return Ok(result);
        }
    }

}

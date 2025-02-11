using DoaMais.Application.Commands.AuthCommands.LoginCommand;
using DoaMais.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoaMais.API.Controllers
{
    /// <summary>
    /// Auth Controller
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IMediator mediator)
        : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var command = new LoginCommand(loginDTO);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }
    }

}

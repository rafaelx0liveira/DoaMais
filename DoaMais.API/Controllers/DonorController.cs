using DoaMais.Application.Commands.DonorCommands.CreateDonorCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoaMais.API.Controllers
{
    /// <summary>
    /// Donor Controller
    /// </summary>
    [ApiController]
    [Route("api/donor")]
    public class DonorController(IMediator mediator)
        : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateDonorCommand createDonorCommand)
        {
            var result = await _mediator.Send(createDonorCommand);
            if(!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }
    }
}

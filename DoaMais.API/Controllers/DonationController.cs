using DoaMais.Application.Commands.DonationCommands.CreateDonationCommand;
using DoaMais.Application.Queries.DonationQueries.GetLastDonationQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoaMais.API.Controllers
{
    /// <summary>
    /// Donation Controller
    /// </summary>
    [Route("api/donation")]
    [ApiController]
    public class DonationController (IMediator mediator) 
        : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateDonationCommand createDonationCommand)
        {
            var result = await _mediator.Send(createDonationCommand);
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLastDonation([FromQuery] Guid donorId)
        {
            var result = await _mediator.Send(new GetLastDonationQuery(donorId));
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }
    }
}

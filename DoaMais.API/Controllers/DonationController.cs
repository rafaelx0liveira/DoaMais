using DoaMais.Application.Commands.DonationCommands.CreateDonationCommand;
using DoaMais.Application.Queries.DonationQueries.GetLastDonationQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace DoaMais.API.Controllers
{
    /// <summary>
    /// Donation Controller
    /// </summary>
    [Route("api/donation")]
    [ApiController]
    public class DonationController (IMediator mediator, ILogger logger)
        : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger _logger = logger;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateDonationCommand createDonationCommand)
        {
            _logger.Information($"Donation request received: DonorId: {createDonationCommand.DonorId}, DonationDate: {createDonationCommand.DonationDate}, QuantityML: {createDonationCommand.QuantityML}");

            var result = await _mediator.Send(createDonationCommand);

            if (!result.IsSuccess)
            {
                _logger.Warning($"Donation request failed: DonorId: {createDonationCommand.DonorId}, DonationDate: {createDonationCommand.DonationDate}, QuantityML: {createDonationCommand.QuantityML}. Reason: {result.Message}");
                return BadRequest(result.Message);
            }

            _logger.Information($"Donation request successful: Donation ID: {result.Data}");
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLastDonation([FromQuery] Guid donorId)
        {
            var result = await _mediator.Send(new GetLastDonationQuery(donorId));

            if (!result.IsSuccess)
            {
                _logger.Warning($"Failed to get last donation for donor: {donorId}. Reason: {result.Message}");
                return BadRequest(result.Message); 
            }

            _logger.Information($"Last donation retrieved for donor: {donorId}");
            return Ok(result);
        }
    }
}

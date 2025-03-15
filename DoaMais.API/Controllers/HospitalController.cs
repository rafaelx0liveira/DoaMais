using DoaMais.Application.Commands.HospitalCommands.CreateHospitalCommand;
using DoaMais.Application.Queries.HospitalQueries.GetAllHospitalsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace DoaMais.API.Controllers
{
    /// <summary>
    /// Hospital Controller
    /// </summary>
    [Route("api/hospital")]
    [ApiController]
    public class HospitalController(IMediator mediator, ILogger logger)
        : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger _logger = logger;

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] CreateHospitalCommand request)
        {
            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                _logger.Warning($"Error creating hospital: {result.Message}");
                return BadRequest(result);
            }

            _logger.Information($"Hospital created: {result.Data}");
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var hospitals = await _mediator.Send(new GetAllHospitalsQuery());
            return Ok(hospitals);
        }
    }
}

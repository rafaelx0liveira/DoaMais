using DoaMais.Application.Commands.DonorCommands.CreateDonorCommand;
using DoaMais.Application.Queries.DonorsQuerys.GetAllDonorsQuery;
using DoaMais.Application.Queries.DonorsQuerys.GetDonorByIdQuery;
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

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetDonorByIdQuery(id));
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var donors = await _mediator.Send(new GetAllDonorsQuery());
            return Ok(donors);
        }
    }
}

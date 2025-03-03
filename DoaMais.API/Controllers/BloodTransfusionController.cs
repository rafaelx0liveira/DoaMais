using DoaMais.Application.Commands.EmployeeCommands.CreateEmployeeCommand;
using DoaMais.Application.Queries.EmployeesQueries.GetAllEmployeesQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoaMais.API.Controllers
{
    /// <summary>
    /// Blood Transfusion Controller
    /// </summary>
    [Route("api/transfusion")]
    [ApiController]
    public class BloodTransfusionController(IMediator mediator)
            : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateEmployeeCommand request)
        {
            var result = await _mediator.Send(request);

            if (!result.IsSuccess) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _mediator.Send(new GetAllEmployeesQuery());
            return Ok(employees);
        }
    }
}

using DoaMais.Application.Commands.EmployeeCommands.CreateEmployeeCommand;
using DoaMais.Application.Queries.EmployeesQueries.GetAllEmployeesQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace DoaMais.API.Controllers
{
    /// <summary>
    /// Employee Controller
    /// </summary>
    [Route("api/employee")]
    [ApiController]
    public class EmployeeController(IMediator mediator, ILogger logger)
        : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger _logger = logger;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateEmployeeCommand request)
        {
            var result = await _mediator.Send(request);

            if(!result.IsSuccess)
            {
                _logger.Warning($"Error creating employee: {result.Message}");
                return BadRequest(result); 
            }

            _logger.Information($"Employee created: {result.Data}");
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

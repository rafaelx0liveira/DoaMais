using DoaMais.Application.Commands.EmployeeCommands.CreateEmployeeCommand;
using DoaMais.Application.DTOs;
using DoaMais.Application.Queries.EmployeesQuerys.GetAllEmployeesQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoaMais.API.Controllers
{
    /// <summary>
    /// Employee Controller
    /// </summary>
    [Route("api/employee")]
    [ApiController]
    public class EmployeeController(IMediator mediator)
        : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO employeeDTO)
        {
            var command = new CreateEmployeeCommand(employeeDTO);
            var result = await _mediator.Send(command);

            if(!result.IsSuccess) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _mediator.Send(new GetAllEmployeesQuery());
            return Ok(employees);
        }
    }
}

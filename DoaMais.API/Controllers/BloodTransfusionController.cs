using DoaMais.Application.Commands.BloodTransfusionCommands.CreateBloodTransfusionCommand;
using DoaMais.Application.Queries.BloodTransfusionsQueries.GetAllBloodTransfusionsQuery;
using DoaMais.Application.Queries.EmployeesQueries.GetAllEmployeesQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace DoaMais.API.Controllers
{
    /// <summary>
    /// Blood Transfusion Controller
    /// </summary>
    [Route("api/transfusion")]
    [ApiController]
    public class BloodTransfusionController(IMediator mediator, ILogger logger)
            : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger _logger = logger;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateBloodTransfusionCommand request)
        {
            _logger.Information($"Blood transfusion request received: CNPJ: {request.CNPJ}, Quantity: {request.QuantityML}, Type: {request.BloodType}, RH Factor: {request.RHFactor}");

            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                _logger.Warning($"Blood transfusion request failed: CNPJ: {request.CNPJ}, Quantity: {request.QuantityML}, Type: {request.BloodType}, RH Factor: {request.RHFactor}. Reason: {result.Message}");
                return BadRequest(result); 
            }

            _logger.Information($"Blood transfusion request successful: Transfusion ID: {result.Data}");
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var bloodTransfusions = await _mediator.Send(new GetAllBloodTransfusionsQuery());

            return Ok(bloodTransfusions);
        }
    }
}

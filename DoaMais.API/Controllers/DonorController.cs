﻿using DoaMais.Application.Commands.DonorCommands.CreateDonorCommand;
using DoaMais.Application.Queries.DonorsQueries.GetAllDonorsQuery;
using DoaMais.Application.Queries.DonorsQueries.GetDonorByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace DoaMais.API.Controllers
{
    /// <summary>
    /// Donor Controller
    /// </summary>
    [ApiController]
    [Route("api/donor")]
    public class DonorController(IMediator mediator, ILogger logger)
        : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger _logger = logger;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateDonorCommand createDonorCommand)
        {
            var result = await _mediator.Send(createDonorCommand);

            if (!result.IsSuccess)
            {
                _logger.Warning($"Error creating donor: {result.Message}");
                return BadRequest(result.Message);
            }

            _logger.Information($"Donor created: {result.Data}");
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetDonorByIdQuery(id));

            if (!result.IsSuccess)
            {
                _logger.Warning($"Error getting donor: {result.Message}");
                return BadRequest(result.Message);
            }

            _logger.Information($"Donor retrieved: {result.Data}");
            return Ok(result);
        }

        [HttpGet("getAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var donors = await _mediator.Send(new GetAllDonorsQuery());
            return Ok(donors);
        }
    }
}

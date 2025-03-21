using DoaMais.Application.Queries.ReportQueries.GetBloodStockReportQuery;
using DoaMais.Application.Queries.ReportQueries.GetDonationReportQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace DoaMais.API.Controllers
{
    /// <summary>
    /// BloodStocks and Donations reports controller
    /// </summary>
    [Route("api/report")]
    [ApiController]
    public class ReportController (IMediator mediator, ILogger logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger _logger = logger;

        [HttpGet("bloodstocks")]
        public async Task<IActionResult> GetBloodStocks()
        {
            var result = await _mediator.Send(new GetBloodStockReportQuery());

            if (!result.IsSuccess || result.Data.FileData == null)
            {
                _logger.Warning($"Error getting blood stocks: {result.Message}");
                return BadRequest(result);
            }

            _logger.Information($"Blood stocks retrieved: {result.Data}");

            return File(result.Data.FileData, "application/pdf");
        }

        [HttpGet("donations")]
        public async Task<IActionResult> GetDonations()
        {
            var result = await _mediator.Send(new GetDonationReportQuery());

            if (!result.IsSuccess || result.Data.FileData == null)
            {
                _logger.Warning($"Error getting donations: {result.Message}");
                return BadRequest(result);
            }

            _logger.Information($"Donations retrieved: {result.Data}");

            return File(result.Data.FileData, "application/pdf");
        }
    }
}

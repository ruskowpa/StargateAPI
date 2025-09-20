using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Queries;
using StargateAPI.Controllers;

namespace StargateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetLogsResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLogs([FromQuery] int? limit = 50, [FromQuery] string? logLevel = null, [FromQuery] string? source = null)
        {
            try
            {
                var result = await _mediator.Send(new GetLogs
                {
                    Limit = limit,
                    LogLevel = logLevel,
                    Source = source
                });
                return this.GetResponse(result);
            }
            catch (System.Exception ex)
            {
                return this.GetResponse(new GetLogsResult 
                { 
                    Success = false, 
                    Message = $"An error occurred: {ex.Message}", 
                    ResponseCode = 500 
                });
            }
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Queries;

namespace StargateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReferenceDataController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReferenceDataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets all available ranks
        /// </summary>
        /// <returns>List of available ranks</returns>
        [HttpGet("ranks")]
        public async Task<IActionResult> GetRanks()
        {
            try
            {
                var result = await _mediator.Send(new GetRanks());
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        /// <summary>
        /// Gets all available duty titles
        /// </summary>
        /// <returns>List of available duty titles</returns>
        [HttpGet("duty-titles")]
        public async Task<IActionResult> GetDutyTitles()
        {
            try
            {
                var result = await _mediator.Send(new GetDutyTitles());
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}


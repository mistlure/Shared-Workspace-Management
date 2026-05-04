using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusHistoryController : ControllerBase
    {
        private readonly IStatusHistoryRepository _statusHistoryRepository;

        public StatusHistoryController(IStatusHistoryRepository statusHistoryRepository)
        {
            _statusHistoryRepository = statusHistoryRepository;
        }



        [HttpGet("workplace/{workplaceId}")]
        public async Task<IActionResult> GetByWorkplace(int workplaceId)
        {
            var history = await _statusHistoryRepository.GetByWorkplaceIdAsync(workplaceId);
            // If no history is found, we can return an empty list or a not found message. Here, we'll return an empty list.
            return Ok(history);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StatusHistory history)
        {
            if (history == null) return BadRequest();

            await _statusHistoryRepository.AddAsync(history);

            return Ok(new { message = "Status history created successfully!" });
        }
    }
}

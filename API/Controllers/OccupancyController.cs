using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OccupancyController : ControllerBase
    {
        private readonly IOccupancyRepository _occupancyRepository;

        public OccupancyController(IOccupancyRepository occupancyRepository)
        {
            _occupancyRepository = occupancyRepository;
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var occupancy = await _occupancyRepository.GetByIdAsync(id);

            if (occupancy == null) return NotFound(new { message = $"Occupancy with ID {id} not found." });

            return Ok(occupancy);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActive()
        {
            var activeOccupancies = await _occupancyRepository.GetAllActiveAsync();

            return Ok(activeOccupancies);
        }

        [HttpGet("user/{userId}/active")]
        public async Task<IActionResult> GetActiveByUserId(int userId)
        {
            var activeOccupancy = await _occupancyRepository.GetActiveByUserIdAsync(userId);

            if (activeOccupancy == null) return NotFound(new { message = $"No active occupancy found for user ID {userId}." });

            return Ok(activeOccupancy);
        }

        [HttpGet("workplace/{workplaceId}/active")]
        public async Task<IActionResult> GetActiveByWorkplaceId(int workplaceId)
        {
            var activeOccupancy = await _occupancyRepository.GetActiveByWorkplaceIdAsync(workplaceId);

            if (activeOccupancy == null) return NotFound(new { message = $"No active occupancy found for workplace ID {workplaceId}." });

            return Ok(activeOccupancy);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Occupancy occupancy)
        {
            if (occupancy == null) return BadRequest("Invalid occupancy data.");

            // Later...

            int newId = await _occupancyRepository.AddAsync(occupancy);

            return Ok(new { id = newId, message = "Occupancy created successfully!" });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Occupancy occupancy)
        {
            if (occupancy == null) return BadRequest("Invalid occupancy data.");

            var existingOccupancy = await _occupancyRepository.GetByIdAsync(occupancy.Id);
            if (existingOccupancy == null) return NotFound(new { message = $"Occupancy with ID {occupancy.Id} not found." });

            await _occupancyRepository.UpdateAsync(occupancy);

            return NoContent();
        }
    }
}

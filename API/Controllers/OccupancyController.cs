using API.Services;
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
        private readonly IOccupancyService _occupancyService;

        public OccupancyController(IOccupancyRepository occupancyRepository, IOccupancyService occupancyService)
        {
            _occupancyRepository = occupancyRepository;
            _occupancyService = occupancyService;
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

            try
            {
                var createdOccupancy = await _occupancyService.CreateOccupancyAsync(occupancy);

                return Ok(new { id = createdOccupancy.Id, message = "Occupancy created successfully!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
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

        [HttpPost("{id}/finish")]
        public async Task<IActionResult> FinishOccupancy(int id)
        {
            try
            {
                await _occupancyService.FinishOccupancyAsync(id);
                return Ok(new { message = $"Occupancy {id} successfully finished and workplace is now available." });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}

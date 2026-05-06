using API.DTOs;
using API.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //[Authorize]
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

            var response = new OccupancyResponseDto
            {
                Id = occupancy.Id,
                UserId = occupancy.UserId,
                WorkplaceId = occupancy.WorkplaceId,
                StartTime = occupancy.StartTime,
                EndTime = occupancy.EndTime,
                TotalPrice = occupancy.TotalPrice
            };

            return Ok(response);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActive()
        {
            var activeOccupancies = await _occupancyRepository.GetAllActiveAsync();

            var response = activeOccupancies.Select(o => new OccupancyResponseDto
            {
                Id = o.Id,
                UserId = o.UserId,
                WorkplaceId = o.WorkplaceId,
                StartTime = o.StartTime,
                EndTime = o.EndTime,
                TotalPrice = o.TotalPrice
            }).ToList();

            return Ok(response);
        }

        [HttpGet("user/{userId}/active")]
        public async Task<IActionResult> GetActiveByUserId(int userId)
        {
            var occupancy = await _occupancyRepository.GetActiveByUserIdAsync(userId);

            if (occupancy == null) return NotFound(new { message = $"No active occupancy found for user ID {userId}." });

            var response = new OccupancyResponseDto
            {
                Id = occupancy.Id,
                UserId = occupancy.UserId,
                WorkplaceId = occupancy.WorkplaceId,
                StartTime = occupancy.StartTime,
                EndTime = occupancy.EndTime,
                TotalPrice = occupancy.TotalPrice
            };

            return Ok(response);
        }

        [HttpGet("workplace/{workplaceId}/active")]
        public async Task<IActionResult> GetActiveByWorkplaceId(int workplaceId)
        {
            var occupancy = await _occupancyRepository.GetActiveByWorkplaceIdAsync(workplaceId);

            if (occupancy == null) return NotFound(new { message = $"No active occupancy found for workplace ID {workplaceId}." });

            var response = new OccupancyResponseDto
            {
                Id = occupancy.Id,
                UserId = occupancy.UserId,
                WorkplaceId = occupancy.WorkplaceId,
                StartTime = occupancy.StartTime,
                EndTime = occupancy.EndTime,
                TotalPrice = occupancy.TotalPrice
            };

            return Ok(response);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var activeOccupancies = await _occupancyRepository.GetAllActiveAsync();

            var response = activeOccupancies
                .Where(o => o.UserId == userId)
                .Select(o => new OccupancyResponseDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    WorkplaceId = o.WorkplaceId,
                    StartTime = o.StartTime,
                    EndTime = o.EndTime,
                    TotalPrice = o.TotalPrice
                }).ToList();

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateOccupancyDto dto)
        {
            if (dto == null) return BadRequest("Invalid occupancy data.");

            try
            {
                var occupancy = new Occupancy
                {
                    UserId = dto.UserId,
                    WorkplaceId = dto.WorkplaceId,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime
                };

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
        public async Task<IActionResult> Update([FromBody] UpdateOccupancyDto dto)
        {
            if (dto == null) return BadRequest("Invalid occupancy data.");

            var existingOccupancy = await _occupancyRepository.GetByIdAsync(dto.Id);
            if (existingOccupancy == null) return NotFound(new { message = $"Occupancy with ID {dto.Id} not found." });

            existingOccupancy.UserId = dto.UserId;
            existingOccupancy.WorkplaceId = dto.WorkplaceId;
            existingOccupancy.StartTime = dto.StartTime;
            existingOccupancy.EndTime = dto.EndTime;
            existingOccupancy.TotalPrice = dto.TotalPrice;

            await _occupancyRepository.UpdateAsync(existingOccupancy);

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

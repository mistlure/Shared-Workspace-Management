using System.Linq;
using API.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
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
            var response = history.Select(h => new StatusHistoryResponseDto
            {
                Id = h.Id,
                WorkplaceId = h.WorkplaceId,
                Status = h.Status,
                ChangedAt = h.ChangedAt
            }).ToList();

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStatusHistoryDto dto)
        {
            if (dto == null) return BadRequest();

            var history = new StatusHistory
            {
                WorkplaceId = dto.WorkplaceId,
                Status = dto.Status
            };

            await _statusHistoryRepository.AddAsync(history);

            return Ok(new { message = "Status history created successfully!" });
        }
    }
}

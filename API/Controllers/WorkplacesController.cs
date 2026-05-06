using API.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkplacesController : ControllerBase
    {
        private readonly IWorkplaceRepository _workplaceRepository;

        public WorkplacesController(IWorkplaceRepository workplaceRepository)
        {
            _workplaceRepository = workplaceRepository;
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var workplaces = await _workplaceRepository.GetAllAsync();

            var response = workplaces.Select(w => new WorkplaceResponseDto
            {
                Id = w.Id,
                WorkspaceId = w.WorkspaceId,
                Name = w.Name,
                CurrentStatus = w.CurrentStatus
            }).ToList();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var workplace = await _workplaceRepository.GetByIdAsync(id);

            if (workplace == null) return NotFound(new { message = $"Workplace with ID {id} not found." });

            var response = new WorkplaceResponseDto
            {
                Id = workplace.Id,
                WorkspaceId = workplace.WorkspaceId,
                Name = workplace.Name,
                CurrentStatus = workplace.CurrentStatus
            };

            return Ok(response);
        }

        [HttpGet("workspace/{workspaceId}")]
        public async Task<IActionResult> GetByWorkspaceId(int workspaceId)
        {
            var allWorkplaces = await _workplaceRepository.GetAllAsync();

            var response = allWorkplaces
                .Where(w => w.WorkspaceId == workspaceId)
                .Select(w => new WorkplaceResponseDto
                {
                    Id = w.Id,
                    WorkspaceId = w.WorkspaceId,
                    Name = w.Name,
                    CurrentStatus = w.CurrentStatus
                }).ToList();

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWorkplaceDto dto)
        {
            if (dto == null) return BadRequest();

            var workplace = new Workplace
            {
                WorkspaceId = dto.WorkspaceId,
                Name = dto.Name,
                CurrentStatus = WorkplaceStatus.Available
            };

            int newId = await _workplaceRepository.AddAsync(workplace);

            var response = new WorkplaceResponseDto
            {
                Id = newId,
                WorkspaceId = workplace.WorkspaceId,
                Name = workplace.Name,
                CurrentStatus = workplace.CurrentStatus
            };

            return Ok(new { data = response, message = "Workplace created successfully!" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WorkplaceResponseDto dto)
        {
            if (dto == null || id != dto.Id) return BadRequest();

            var existing = await _workplaceRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.WorkspaceId = dto.WorkspaceId;
            existing.Name = dto.Name;
            existing.CurrentStatus = dto.CurrentStatus;

            await _workplaceRepository.UpdateAsync(existing);
            return Ok(new { message = "Workplace updated!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _workplaceRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _workplaceRepository.DeleteAsync(id);
            return Ok(new { message = "Workplace deleted!" });
        }
    }
}

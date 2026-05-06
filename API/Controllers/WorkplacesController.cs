using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using Domain.Enums;

namespace API.Controllers
{
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
    }
}

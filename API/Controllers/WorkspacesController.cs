using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceRepository _workspaceRepository;

        public WorkspacesController(IWorkspaceRepository workspaceRepository)
        {
            _workspaceRepository = workspaceRepository;
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var workspaces = await _workspaceRepository.GetAllAsync();

            var response = workspaces.Select(w => new WorkspaceResponseDto
            {
                Id = w.Id,
                Name = w.Name,
                Location = w.Location,
                MaxOccupationHours = w.MaxOccupationHours,
                PricePerHour = w.PricePerHour
            }).ToList();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var workspace = await _workspaceRepository.GetByIdAsync(id);

            if (workspace == null) return NotFound(new { message = $"Workspace with ID {id} not found." });

            var response = new WorkspaceResponseDto
            {
                Id = workspace.Id,
                Name = workspace.Name,
                Location = workspace.Location,
                MaxOccupationHours = workspace.MaxOccupationHours,
                PricePerHour = workspace.PricePerHour
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWorkspaceDto dto)
        {
            if (dto == null) return BadRequest();

            var workspace = new Workspace
            {
                Name = dto.Name,
                Location = dto.Location,
                MaxOccupationHours = dto.MaxOccupationHours,
                PricePerHour = dto.PricePerHour
            };

            int newId = await _workspaceRepository.AddAsync(workspace);

            var response = new WorkspaceResponseDto
            {
                Id = newId,
                Name = workspace.Name,
                Location = workspace.Location,
                MaxOccupationHours = workspace.MaxOccupationHours,
                PricePerHour = workspace.PricePerHour
            };

            return Ok(new { data = response, message = "Workspace created successfully!" });
        }
    }
}

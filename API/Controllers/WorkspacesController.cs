using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

            return Ok(workspaces);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var workspace = await _workspaceRepository.GetByIdAsync(id);

            if (workspace == null) return NotFound(new { message = $"Workspace with ID {id} not found." });

            return Ok(workspace);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Workspace workspace)
        {
            if (workspace == null) return BadRequest();

            int newId = await _workspaceRepository.AddAsync(workspace);

            return Ok(new { id = newId, message = "Workspace created successfully!" });
        }
    }
}

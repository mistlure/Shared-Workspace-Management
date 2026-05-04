using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

            return Ok(workplaces);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var workplace = await _workplaceRepository.GetByIdAsync(id);

            if (workplace == null) return NotFound(new { message = $"Workplace with ID {id} not found." });

            return Ok(workplace);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Workplace workplace)
        {
            if (workplace == null) return BadRequest();

            int newId = await _workplaceRepository.AddAsync(workplace);

            return Ok(new { id = newId, message = "Workplace created successfully!" });
        }
    }
}

using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // ControllerBase gives us access to features like model binding, validation, and HTTP response handling.
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (user == null) return BadRequest();
            user.CreatedAt = DateTime.UtcNow;
            int newId = await _userRepository.AddAsync(user);
            return Ok(new { id = newId, message = "User created successfully!" });
        }
    }
}

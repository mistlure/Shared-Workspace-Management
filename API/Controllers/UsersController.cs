using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    // ControllerBase gives us access to features like model binding, validation, and HTTP response handling.
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public UsersController(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null) return NotFound(new { message = $"User with ID {id} not found." });

            var response = new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllAsync();

            var response = users.Select(user => new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            }).ToList();

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserRegisterDto dto)
        {
            if (dto == null) return BadRequest();

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = dto.Password,
                CreatedAt = DateTime.Now
            };

            int newId = await _userRepository.AddAsync(user);

            var response = new UserResponseDto
            {
                Id = newId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };

            return Ok(new { data = response, message = "User created successfully!" });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (dto == null) return BadRequest();

            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == dto.Email && u.PasswordHash == dto.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var response = new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };

            var tokenString = GenerateJwtToken(user);

            return Ok(new
            {
                token = tokenString,
                data = response,
                message = "Login successful!"
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserResponseDto dto)
        {
            if (dto == null || id != dto.Id) return BadRequest(new { message = "Invalid data" });

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) return NotFound(new { message = "User not found" });

            existingUser.FirstName = dto.FirstName;
            existingUser.LastName = dto.LastName;
            existingUser.Email = dto.Email;

            await _userRepository.UpdateAsync(existingUser);

            return Ok(new { message = "User updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userRepository.DeleteAsync(id);
            return Ok();
        }


        // ------------------------


        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("FirstName", user.FirstName)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

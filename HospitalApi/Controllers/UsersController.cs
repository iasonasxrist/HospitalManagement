using Microsoft.AspNetCore.Mvc;
using HospitalApi.Models;
using HospitalApi.Application.DTOs;
using HospitalApi.Authorization;
using HospitalApi.Infrastructure.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace HospitalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: api/users
        [HttpGet]
        [AuthorizeRoles(UserRole.Admin)]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers([FromQuery] UserRole? role = null)
        {
            _logger.LogInformation("GET /api/users called with role filter: {Role}", role);
            
            var users = await _userService.GetUsersAsync(role);
            _logger.LogInformation("Returning {Count} users", users.Count());
            
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        [AuthorizeRoles(UserRole.Admin, UserRole.Doctor, UserRole.Nurse)]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            _logger.LogInformation("GET /api/users/{Id} called", id);
            
            var user = await _userService.GetUserAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Returning user: {Username}", user.Username);
            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        [AuthorizeRoles(UserRole.Admin)]
        public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto dto)
        {
            _logger.LogInformation("POST /api/users called for username: {Username}", dto.Username);
            
            try
            {
                var user = await _userService.CreateUserAsync(dto);
                _logger.LogInformation("Successfully created user with ID: {UserId}", user.Id);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Failed to create user: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        [AuthorizeRoles(UserRole.Admin)]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var success = await _userService.UpdateUserAsync(id, dto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        [AuthorizeRoles(UserRole.Admin)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDto>> Login(LoginDto dto)
        {
            _logger.LogInformation("POST /api/users/login called for username: {Username}", dto.Username);
            
            var user = await _userService.LoginAsync(dto);
            if (user == null)
            {
                _logger.LogWarning("Login failed for username: {Username}", dto.Username);
                return Unauthorized("Invalid username or password");
            }

            _logger.LogInformation("Successful login for user: {Username}", dto.Username);
            return Ok(user);
        }

        // GET: api/users/doctors
        [HttpGet("doctors")]
        [AuthorizeRoles(UserRole.Admin, UserRole.Doctor, UserRole.Nurse)]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetDoctors()
        {
            var doctors = await _userService.GetDoctorsAsync();
            return Ok(doctors);
        }

        // GET: api/users/nurses
        [HttpGet("nurses")]
        [AuthorizeRoles(UserRole.Admin, UserRole.Doctor, UserRole.Nurse)]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetNurses()
        {
            var nurses = await _userService.GetNursesAsync();
            return Ok(nurses);
        }
    }
} 
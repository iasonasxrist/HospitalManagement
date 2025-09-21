using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HospitalApi.Models;
using HospitalApi.Application.DTOs;
using HospitalApi.Infrastructure.Interfaces.Services;
using HospitalApi.Infrastructure.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace HospitalApi.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersAsync(UserRole? role = null)
        {
            _logger.LogInformation("Getting users with role filter: {Role}", role);
            
            IEnumerable<User> users;
            if (role.HasValue)
                users = await _userRepository.GetByRoleAsync(role.Value);
            else
                users = await _userRepository.GetAllAsync();
            
            var activeUsers = users.Where(u => u.IsActive).Select(u => ToResponseDto(u));
            _logger.LogInformation("Retrieved {Count} active users", activeUsers.Count());
            
            return activeUsers;
        }

        public async Task<UserResponseDto?> GetUserAsync(int id)
        {
            _logger.LogInformation("Getting user with ID: {UserId}", id);
            
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return null;
            }
            
            if (!user.IsActive)
            {
                _logger.LogWarning("User with ID {UserId} is inactive", id);
                return null;
            }
            
            _logger.LogInformation("Successfully retrieved user: {Username}", user.Username);
            return ToResponseDto(user);
        }

        public async Task<UserResponseDto?> CreateUserAsync(CreateUserDto dto)
        {
            _logger.LogInformation("Creating new user with username: {Username}", dto.Username);
            
            if (await _userRepository.GetByUsernameAsync(dto.Username) != null)
            {
                _logger.LogWarning("Username already exists: {Username}", dto.Username);
                throw new ArgumentException("Username already exists");
            }
            
            if (await _userRepository.GetByEmailAsync(dto.Email) != null)
            {
                _logger.LogWarning("Email already exists: {Email}", dto.Email);
                throw new ArgumentException("Email already exists");
            }
            
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = dto.Role,
                PhoneNumber = dto.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            await _userRepository.AddAsync(user);
            _logger.LogInformation("Successfully created user with ID: {UserId}", user.Id);
            
            return ToResponseDto(user);
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || !user.IsActive)
                return false;
            if (dto.FirstName != null)
                user.FirstName = dto.FirstName;
            if (dto.LastName != null)
                user.LastName = dto.LastName;
            if (dto.PhoneNumber != null)
                user.PhoneNumber = dto.PhoneNumber;
            if (dto.IsActive.HasValue)
                user.IsActive = dto.IsActive.Value;
            user.LastUpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;
            user.IsActive = false;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<UserResponseDto?> LoginAsync(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for username: {Username}", dto.Username);
            
            var user = await _userRepository.GetByUsernameAsync(dto.Username);
            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found: {Username}", dto.Username);
                return null;
            }
            
            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed - user is inactive: {Username}", dto.Username);
                return null;
            }
            
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed - invalid password for user: {Username}", dto.Username);
                return null;
            }
            
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Successful login for user: {Username}", dto.Username);
            
            return ToResponseDto(user);
        }

        public async Task<IEnumerable<UserResponseDto>> GetDoctorsAsync()
        {
            var doctors = await _userRepository.GetByRoleAsync(UserRole.Doctor);
            return doctors.Where(u => u.IsActive).Select(u => ToResponseDto(u));
        }

        public async Task<IEnumerable<UserResponseDto>> GetNursesAsync()
        {
            var nurses = await _userRepository.GetByRoleAsync(UserRole.Nurse);
            return nurses.Where(u => u.IsActive).Select(u => ToResponseDto(u));
        }

        private static UserResponseDto ToResponseDto(User user) => new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive
        };
    }
} 
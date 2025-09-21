using HospitalApi.Application.DTOs;
using HospitalApi.Models;

namespace HospitalApi.Infrastructure.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetUsersAsync(UserRole? role = null);
        Task<UserResponseDto?> GetUserAsync(int id);
        Task<UserResponseDto?> CreateUserAsync(CreateUserDto dto);
        Task<bool> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<UserResponseDto?> LoginAsync(LoginDto dto);
        Task<IEnumerable<UserResponseDto>> GetDoctorsAsync();
        Task<IEnumerable<UserResponseDto>> GetNursesAsync();
    }
} 
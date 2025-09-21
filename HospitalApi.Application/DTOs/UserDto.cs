using System.ComponentModel.DataAnnotations;
using HospitalApi.Models;

namespace HospitalApi.Application.DTOs
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public UserRole Role { get; set; }
        
        public string? PhoneNumber { get; set; }
    }
    
    public class UpdateUserDto
    {
        [StringLength(50)]
        public string? FirstName { get; set; }
        
        [StringLength(50)]
        public string? LastName { get; set; }
        
        public string? PhoneNumber { get; set; }
        
        public bool? IsActive { get; set; }
    }
    
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
    }
    
    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
} 
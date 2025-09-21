using System.ComponentModel.DataAnnotations;

namespace HospitalApi.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public UserRole Role { get; set; }
        
        public string? PhoneNumber { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastLoginAt { get; set; }
        
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
    
    public enum UserRole
    {
        Admin,
        Doctor,
        Nurse
    }
} 
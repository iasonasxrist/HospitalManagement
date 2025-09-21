using System.ComponentModel.DataAnnotations;
using HospitalApi.Models;

namespace HospitalApi.Application.DTOs
{
    public class CreatePatientDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? EmergencyContact { get; set; }
        
        [StringLength(20)]
        public string? EmergencyPhone { get; set; }
        
        [StringLength(500)]
        public string? MedicalHistory { get; set; }
        
        [StringLength(500)]
        public string? Allergies { get; set; }
    }
    
    public class UpdatePatientDto
    {
        [StringLength(50)]
        public string? FirstName { get; set; }
        
        [StringLength(50)]
        public string? LastName { get; set; }
        
        [StringLength(200)]
        public string? Address { get; set; }
        
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        
        [StringLength(100)]
        public string? EmergencyContact { get; set; }
        
        [StringLength(20)]
        public string? EmergencyPhone { get; set; }
        
        [StringLength(500)]
        public string? MedicalHistory { get; set; }
        
        [StringLength(500)]
        public string? Allergies { get; set; }
        
        public PatientStatus? Status { get; set; }
        
        public bool? IsCritical { get; set; }
    }
    
    public class PatientResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        public string? MedicalHistory { get; set; }
        public string? Allergies { get; set; }
        public PatientStatus Status { get; set; }
        public bool IsCritical { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
} 
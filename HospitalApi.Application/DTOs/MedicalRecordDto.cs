using System.ComponentModel.DataAnnotations;
using HospitalApi.Models;

namespace HospitalApi.Application.DTOs
{
    public class CreateMedicalRecordDto
    {
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int DoctorId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Diagnosis { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Symptoms { get; set; }
        
        [StringLength(1000)]
        public string? Treatment { get; set; }
        
        [StringLength(500)]
        public string? Prescriptions { get; set; }
        
        public decimal? Temperature { get; set; }
        
        public int? BloodPressureSystolic { get; set; }
        
        public int? BloodPressureDiastolic { get; set; }
        
        public int? HeartRate { get; set; }
        
        public decimal? Weight { get; set; }
        
        public decimal? Height { get; set; }
        
        public bool IsCritical { get; set; } = false;
        
        [StringLength(500)]
        public string? CriticalNotes { get; set; }
    }
    
    public class UpdateMedicalRecordDto
    {
        [StringLength(200)]
        public string? Diagnosis { get; set; }
        
        [StringLength(1000)]
        public string? Symptoms { get; set; }
        
        [StringLength(1000)]
        public string? Treatment { get; set; }
        
        [StringLength(500)]
        public string? Prescriptions { get; set; }
        
        public decimal? Temperature { get; set; }
        
        public int? BloodPressureSystolic { get; set; }
        
        public int? BloodPressureDiastolic { get; set; }
        
        public int? HeartRate { get; set; }
        
        public decimal? Weight { get; set; }
        
        public decimal? Height { get; set; }
        
        public bool? IsCritical { get; set; }
        
        [StringLength(500)]
        public string? CriticalNotes { get; set; }
    }
    
    public class MedicalRecordResponseDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string? Symptoms { get; set; }
        public string? Treatment { get; set; }
        public string? Prescriptions { get; set; }
        public decimal? Temperature { get; set; }
        public int? BloodPressureSystolic { get; set; }
        public int? BloodPressureDiastolic { get; set; }
        public int? HeartRate { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public bool IsCritical { get; set; }
        public string? CriticalNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
    }
} 
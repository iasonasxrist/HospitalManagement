using System.ComponentModel.DataAnnotations;
using HospitalApi.Models;

namespace HospitalApi.Application.DTOs
{
    public class CreateVitalSignDto
    {
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int RecordedByUserId { get; set; }
        
        public decimal? Temperature { get; set; }
        
        public int? BloodPressureSystolic { get; set; }
        
        public int? BloodPressureDiastolic { get; set; }
        
        public int? HeartRate { get; set; }
        
        public int? OxygenSaturation { get; set; }
        
        public int? RespiratoryRate { get; set; }
        
        public decimal? Weight { get; set; }
        
        public decimal? Height { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }
    
    public class VitalSignResponseDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int RecordedByUserId { get; set; }
        public decimal? Temperature { get; set; }
        public int? BloodPressureSystolic { get; set; }
        public int? BloodPressureDiastolic { get; set; }
        public int? HeartRate { get; set; }
        public int? OxygenSaturation { get; set; }
        public int? RespiratoryRate { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public VitalSignSeverity Severity { get; set; }
        public string? Notes { get; set; }
        public DateTime RecordedAt { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string RecordedByUserName { get; set; } = string.Empty;
    }
} 
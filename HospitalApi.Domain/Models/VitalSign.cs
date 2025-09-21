using System.ComponentModel.DataAnnotations;

namespace HospitalApi.Models
{
    public class VitalSign
    {
        public int Id { get; set; }
        
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
        
        public VitalSignSeverity Severity { get; set; } = VitalSignSeverity.Normal;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual User RecordedByUser { get; set; } = null!;
    }
    
    public enum VitalSignSeverity
    {
        Normal,
        Elevated,
        High,
        Critical
    }
} 
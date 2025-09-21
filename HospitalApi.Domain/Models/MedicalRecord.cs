using System.ComponentModel.DataAnnotations;

namespace HospitalApi.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        
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
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual User Doctor { get; set; } = null!;
    }
} 
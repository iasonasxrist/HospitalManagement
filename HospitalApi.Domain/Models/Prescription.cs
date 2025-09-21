using System.ComponentModel.DataAnnotations;

namespace HospitalApi.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int PrescribedByUserId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string MedicationName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Dosage { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Frequency { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Instructions { get; set; }
        
        public DateTime PrescribedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Active;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual User PrescribedByUser { get; set; } = null!;
    }
    
    public enum PrescriptionStatus
    {
        Active,
        Discontinued,
        Completed,
        OnHold
    }
} 
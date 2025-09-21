using System.ComponentModel.DataAnnotations;

namespace HospitalApi.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int DoctorId { get; set; }
        
        [Required]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        [StringLength(100)]
        public string AppointmentType { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual User Doctor { get; set; } = null!;
    }
    
    public enum AppointmentStatus
    {
        Scheduled,
        Confirmed,
        InProgress,
        Completed,
        Cancelled,
        NoShow
    }
} 
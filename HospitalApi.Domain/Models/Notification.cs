using System.ComponentModel.DataAnnotations;

namespace HospitalApi.Models
{
    public class Notification
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
        
        [Required]
        public NotificationType Type { get; set; }
        
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
        
        public NotificationSeverity Severity { get; set; } = NotificationSeverity.Normal;
        
        public int? PatientId { get; set; }
        
        public int? UserId { get; set; }
        
        [StringLength(100)]
        public string? Room { get; set; }
        
        [StringLength(50)]
        public string? Department { get; set; }
        
        [StringLength(500)]
        public string? AdditionalData { get; set; }
        
        public bool IsRead { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ReadAt { get; set; }
        
        // Navigation properties
        public virtual Patient? Patient { get; set; }
        public virtual User? User { get; set; }
    }
    
    public enum NotificationType
    {
        CriticalAlert,
        PatientUpdate,
        AppointmentReminder,
        SystemAlert,
        MedicalRecordUpdate,
        VitalSignAlert,
        LabResultAlert,
        MedicationAlert,
        EmergencyAlert
    }
    
    public enum NotificationPriority
    {
        Low,
        Normal,
        High,
        Critical
    }
    
    public enum NotificationSeverity
    {
        Normal,
        Elevated,
        High,
        Critical
    }
} 
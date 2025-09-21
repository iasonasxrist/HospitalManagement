using System.ComponentModel.DataAnnotations;

namespace HospitalApi.Models
{
    public class Patient
    {
        public int Id { get; set; }
        
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
        
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
        
        [StringLength(100)]
        public string? EmergencyContact { get; set; }
        
        [StringLength(20)]
        public string? EmergencyPhone { get; set; }
        
        [StringLength(500)]
        public string? MedicalHistory { get; set; }
        
        [StringLength(500)]
        public string? Allergies { get; set; }
        
        [StringLength(10)]
        public string? BloodType { get; set; }
        
        [StringLength(20)]
        public string? Room { get; set; }
        
        [StringLength(50)]
        public string? Department { get; set; }
        
        [StringLength(100)]
        public string? Condition { get; set; }
        
        public PatientStatus Status { get; set; } = PatientStatus.Active;
        
        public bool IsCritical { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastUpdatedAt { get; set; }
        
        // Navigation properties
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public virtual ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
        public virtual ICollection<ProgressNote> ProgressNotes { get; set; } = new List<ProgressNote>();
    }
    
    public enum PatientStatus
    {
        Active,
        Inactive,
        Discharged,
        Deceased
    }
} 
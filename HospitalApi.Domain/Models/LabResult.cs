using System.ComponentModel.DataAnnotations;

namespace HospitalApi.Models
{
    public class LabResult
    {
        public int Id { get; set; }
        
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int OrderedByUserId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TestName { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? TestValue { get; set; }
        
        [StringLength(50)]
        public string? NormalRange { get; set; }
        
        [StringLength(20)]
        public string? Unit { get; set; }
        
        public LabResultStatus Status { get; set; } = LabResultStatus.Pending;
        
        public LabResultSeverity Severity { get; set; } = LabResultSeverity.Normal;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? CompletedAt { get; set; }
        
        public DateTime? ReportedAt { get; set; }
        
        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual User OrderedByUser { get; set; } = null!;
    }
    
    public enum LabResultStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }
    
    public enum LabResultSeverity
    {
        Normal,
        Elevated,
        High,
        Critical
    }
} 
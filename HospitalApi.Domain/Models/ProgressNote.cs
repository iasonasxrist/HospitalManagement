using System.ComponentModel.DataAnnotations;

namespace HospitalApi.Models
{
    public class ProgressNote
    {
        public int Id { get; set; }
        
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int CreatedByUserId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Category { get; set; }
        
        public ProgressNoteType Type { get; set; } = ProgressNoteType.General;
        
        public bool IsCritical { get; set; } = false;
        
        [StringLength(500)]
        public string? CriticalNotes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
    }
    
    public enum ProgressNoteType
    {
        General,
        Assessment,
        Plan,
        Evaluation,
        Discharge,
        Consultation
    }
} 
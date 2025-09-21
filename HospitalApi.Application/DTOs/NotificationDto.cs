using System.ComponentModel.DataAnnotations;
using HospitalApi.Models;

namespace HospitalApi.Application.DTOs
{
    public class CreateNotificationDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
        
        [Required]
        public NotificationType Type { get; set; }
        
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
        
        public int? PatientId { get; set; }
        
        public int? UserId { get; set; }
    }
    
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public NotificationPriority Priority { get; set; }
        public int? PatientId { get; set; }
        public int? UserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? PatientName { get; set; }
        public string? UserName { get; set; }
    }
    
    public class MarkNotificationReadDto
    {
        public int NotificationId { get; set; }
    }
} 
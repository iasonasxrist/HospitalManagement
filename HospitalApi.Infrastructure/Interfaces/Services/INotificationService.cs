using HospitalApi.Application.DTOs;

namespace HospitalApi.Infrastructure.Interfaces.Services
{
    public interface INotificationService
    {
        Task<NotificationResponseDto> CreateNotificationAsync(CreateNotificationDto dto);
        Task<IEnumerable<NotificationResponseDto>> GetNotificationsAsync(int? userId = null, bool? isRead = null);
        Task<NotificationResponseDto> MarkAsReadAsync(int notificationId);
        Task CreateCriticalPatientAlertAsync(int patientId, string message);
        Task CreateMedicalRecordUpdateNotificationAsync(int patientId, int doctorId, string diagnosis);
        Task CreateAppointmentReminderAsync(int appointmentId);
    }
} 
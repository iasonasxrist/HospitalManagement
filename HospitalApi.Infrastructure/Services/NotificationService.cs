using HospitalApi.Data;
using HospitalApi.Models;
using HospitalApi.Application.DTOs;
using HospitalApi.Infrastructure.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalApi.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HospitalContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(HospitalContext context, ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<NotificationResponseDto> CreateNotificationAsync(CreateNotificationDto dto)
        {
            _logger.LogInformation("Creating notification: {Title} for user ID: {UserId}", dto.Title, dto.UserId);
            
            var notification = new Notification
            {
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                Priority = dto.Priority,
                PatientId = dto.PatientId,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully created notification with ID: {NotificationId}", notification.Id);

            return await GetNotificationResponseDtoAsync(notification);
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetNotificationsAsync(int? userId = null, bool? isRead = null)
        {
            var query = _context.Notifications
                .Include(n => n.Patient)
                .Include(n => n.User)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(n => n.UserId == userId);

            if (isRead.HasValue)
                query = query.Where(n => n.IsRead == isRead.Value);

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications.Select(n => new NotificationResponseDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                Priority = n.Priority,
                PatientId = n.PatientId,
                UserId = n.UserId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt,
                PatientName = n.Patient != null ? $"{n.Patient.FirstName} {n.Patient.LastName}" : null,
                UserName = n.User != null ? $"{n.User.FirstName} {n.User.LastName}" : null
            });
        }

        public async Task<NotificationResponseDto> MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
                throw new ArgumentException("Notification not found");

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetNotificationResponseDtoAsync(notification);
        }

        public async Task CreateCriticalPatientAlertAsync(int patientId, string message)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null)
                throw new ArgumentException("Patient not found");

            // Create notification for all doctors and nurses
            var medicalStaff = await _context.Users
                .Where(u => (u.Role == UserRole.Doctor || u.Role == UserRole.Nurse) && u.IsActive)
                .ToListAsync();

            foreach (var staff in medicalStaff)
            {
                await CreateNotificationAsync(new CreateNotificationDto
                {
                    Title = "Critical Patient Alert",
                    Message = $"Patient {patient.FirstName} {patient.LastName}: {message}",
                    Type = NotificationType.CriticalAlert,
                    Priority = NotificationPriority.Critical,
                    PatientId = patientId,
                    UserId = staff.Id
                });
            }
        }

        public async Task CreateMedicalRecordUpdateNotificationAsync(int patientId, int doctorId, string diagnosis)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            var doctor = await _context.Users.FindAsync(doctorId);

            if (patient == null || doctor == null)
                throw new ArgumentException("Patient or doctor not found");

            await CreateNotificationAsync(new CreateNotificationDto
            {
                Title = "Medical Record Updated",
                Message = $"Dr. {doctor.LastName} updated medical record for {patient.FirstName} {patient.LastName}. Diagnosis: {diagnosis}",
                Type = NotificationType.MedicalRecordUpdate,
                Priority = NotificationPriority.Normal,
                PatientId = patientId,
                UserId = null // Notify all staff
            });
        }

        public async Task CreateAppointmentReminderAsync(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                throw new ArgumentException("Appointment not found");

            await CreateNotificationAsync(new CreateNotificationDto
            {
                Title = "Appointment Reminder",
                Message = $"Reminder: Appointment with {appointment.Patient.FirstName} {appointment.Patient.LastName} on {appointment.AppointmentDate:g}",
                Type = NotificationType.AppointmentReminder,
                Priority = NotificationPriority.Normal,
                PatientId = appointment.PatientId,
                UserId = appointment.DoctorId
            });
        }

        private async Task<NotificationResponseDto> GetNotificationResponseDtoAsync(Notification notification)
        {
            await _context.Entry(notification)
                .Reference(n => n.Patient)
                .LoadAsync();

            await _context.Entry(notification)
                .Reference(n => n.User)
                .LoadAsync();

            return new NotificationResponseDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                Priority = notification.Priority,
                PatientId = notification.PatientId,
                UserId = notification.UserId,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt,
                PatientName = notification.Patient != null ? $"{notification.Patient.FirstName} {notification.Patient.LastName}" : null,
                UserName = notification.User != null ? $"{notification.User.FirstName} {notification.User.LastName}" : null
            };
        }
    }
} 
using HospitalApi.Application.DTOs;
using HospitalApi.Infrastructure.Interfaces.Services;
using HospitalApi.Infrastructure.Interfaces.Repositories;
using HospitalApi.Models;
using Microsoft.Extensions.Logging;

namespace HospitalApi.Infrastructure.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IPatientRepository patientRepository, INotificationService notificationService, ILogger<PatientService> logger)
        {
            _patientRepository = patientRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<IEnumerable<PatientResponseDto>> GetPatientsAsync(bool? isCritical = null, PatientStatus? status = null, string? search = null)
        {
            _logger.LogInformation("Getting patients with filters - Critical: {IsCritical}, Status: {Status}, Search: {Search}", isCritical, status, search);
            
            IEnumerable<Patient> patients;

            if (!string.IsNullOrEmpty(search))
            {
                patients = await _patientRepository.SearchAsync(search);
                _logger.LogInformation("Retrieved {Count} patients matching search term: {Search}", patients.Count(), search);
            }
            else if (isCritical.HasValue && isCritical.Value)
            {
                patients = await _patientRepository.GetCriticalPatientsAsync();
                _logger.LogInformation("Retrieved {Count} critical patients", patients.Count());
            }
            else if (status.HasValue)
            {
                patients = await _patientRepository.GetByStatusAsync(status.Value);
                _logger.LogInformation("Retrieved {Count} patients with status: {Status}", patients.Count(), status);
            }
            else
            {
                patients = await _patientRepository.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} patients", patients.Count());
            }

            return patients.Select(p => ToResponseDto(p));
        }

        public async Task<PatientResponseDto?> GetPatientAsync(int id)
        {
            _logger.LogInformation("Getting patient with ID: {PatientId}", id);
            
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {PatientId} not found", id);
                return null;
            }
            
            _logger.LogInformation("Successfully retrieved patient: {PatientName}", $"{patient.FirstName} {patient.LastName}");
            return ToResponseDto(patient);
        }

        public async Task<PatientResponseDto> CreatePatientAsync(CreatePatientDto dto)
        {
            _logger.LogInformation("Creating new patient: {FirstName} {LastName}", dto.FirstName, dto.LastName);
            
            var patient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                EmergencyContact = dto.EmergencyContact,
                EmergencyPhone = dto.EmergencyPhone,
                MedicalHistory = dto.MedicalHistory,
                Allergies = dto.Allergies,
                Status = PatientStatus.Active,
                IsCritical = false,
                CreatedAt = DateTime.UtcNow
            };

            await _patientRepository.AddAsync(patient);
            _logger.LogInformation("Successfully created patient with ID: {PatientId}", patient.Id);
            
            return ToResponseDto(patient);
        }

        public async Task<bool> UpdatePatientAsync(int id, UpdatePatientDto dto)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return false;

            bool wasCritical = patient.IsCritical;

            if (dto.FirstName != null)
                patient.FirstName = dto.FirstName;
            if (dto.LastName != null)
                patient.LastName = dto.LastName;
            if (dto.Address != null)
                patient.Address = dto.Address;
            if (dto.PhoneNumber != null)
                patient.PhoneNumber = dto.PhoneNumber;
            if (dto.EmergencyContact != null)
                patient.EmergencyContact = dto.EmergencyContact;
            if (dto.EmergencyPhone != null)
                patient.EmergencyPhone = dto.EmergencyPhone;
            if (dto.MedicalHistory != null)
                patient.MedicalHistory = dto.MedicalHistory;
            if (dto.Allergies != null)
                patient.Allergies = dto.Allergies;
            if (dto.Status.HasValue)
                patient.Status = dto.Status.Value;
            if (dto.IsCritical.HasValue)
                patient.IsCritical = dto.IsCritical.Value;

            patient.LastUpdatedAt = DateTime.UtcNow;

            await _patientRepository.UpdateAsync(patient);

            // Create critical alert if patient becomes critical
            if (!wasCritical && patient.IsCritical)
            {
                await _notificationService.CreateCriticalPatientAlertAsync(
                    patient.Id, 
                    $"Patient {patient.FirstName} {patient.LastName} has been marked as critical."
                );
            }

            return true;
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return false;

            patient.Status = PatientStatus.Inactive;
            patient.LastUpdatedAt = DateTime.UtcNow;
            await _patientRepository.UpdateAsync(patient);
            return true;
        }

        public async Task<IEnumerable<PatientResponseDto>> GetCriticalPatientsAsync()
        {
            var patients = await _patientRepository.GetCriticalPatientsAsync();
            return patients.Select(p => ToResponseDto(p));
        }

        public async Task<bool> MarkPatientCriticalAsync(int id, string reason)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null || patient.IsCritical)
                return false;

            patient.IsCritical = true;
            patient.LastUpdatedAt = DateTime.UtcNow;
            await _patientRepository.UpdateAsync(patient);

            await _notificationService.CreateCriticalPatientAlertAsync(
                patient.Id, 
                $"Patient {patient.FirstName} {patient.LastName} marked as critical. Reason: {reason}"
            );

            return true;
        }

        public async Task<bool> MarkPatientStableAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null || !patient.IsCritical)
                return false;

            patient.IsCritical = false;
            patient.LastUpdatedAt = DateTime.UtcNow;
            await _patientRepository.UpdateAsync(patient);

            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                Title = "Patient Status Update",
                Message = $"Patient {patient.FirstName} {patient.LastName} is now stable.",
                Type = NotificationType.PatientUpdate,
                Priority = NotificationPriority.Normal,
                PatientId = patient.Id
            });

            return true;
        }

        private static PatientResponseDto ToResponseDto(Patient patient) => new PatientResponseDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            Address = patient.Address,
            PhoneNumber = patient.PhoneNumber,
            EmergencyContact = patient.EmergencyContact,
            EmergencyPhone = patient.EmergencyPhone,
            MedicalHistory = patient.MedicalHistory,
            Allergies = patient.Allergies,
            Status = patient.Status,
            IsCritical = patient.IsCritical,
            CreatedAt = patient.CreatedAt,
            LastUpdatedAt = patient.LastUpdatedAt
        };
    }
} 
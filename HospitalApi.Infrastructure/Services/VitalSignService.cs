using HospitalApi.Application.DTOs;
using HospitalApi.Infrastructure.Interfaces.Services;
using HospitalApi.Infrastructure.Interfaces.Repositories;
using HospitalApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalApi.Infrastructure.Services
{
    public class VitalSignService : IVitalSignService
    {
        private readonly IVitalSignRepository _vitalSignRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<VitalSignService> _logger;

        public VitalSignService(
            IVitalSignRepository vitalSignRepository,
            IPatientRepository patientRepository,
            IUserRepository userRepository,
            ILogger<VitalSignService> logger)
        {
            _vitalSignRepository = vitalSignRepository;
            _patientRepository = patientRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<VitalSignResponseDto> CreateVitalSignAsync(CreateVitalSignDto dto)
        {
            _logger.LogInformation("Creating vital sign for patient ID: {PatientId}, recorded by user ID: {UserId}", dto.PatientId, dto.RecordedByUserId);
            
            // Validate patient exists
            var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
            if (patient == null)
            {
                _logger.LogError("Patient not found with ID: {PatientId}", dto.PatientId);
                throw new ArgumentException("Patient not found");
            }

            // Validate user exists
            var user = await _userRepository.GetByIdAsync(dto.RecordedByUserId);
            if (user == null)
            {
                _logger.LogError("User not found with ID: {UserId}", dto.RecordedByUserId);
                throw new ArgumentException("User not found");
            }

            // Determine severity
            var severity = DetermineSeverity(dto);
            _logger.LogInformation("Determined vital sign severity: {Severity}", severity);

            // Create vital sign
            var vitalSign = new VitalSign
            {
                PatientId = dto.PatientId,
                RecordedByUserId = dto.RecordedByUserId,
                Temperature = dto.Temperature,
                BloodPressureSystolic = dto.BloodPressureSystolic,
                BloodPressureDiastolic = dto.BloodPressureDiastolic,
                HeartRate = dto.HeartRate,
                OxygenSaturation = dto.OxygenSaturation,
                RespiratoryRate = dto.RespiratoryRate,
                Weight = dto.Weight,
                Height = dto.Height,
                Severity = severity,
                Notes = dto.Notes,
                RecordedAt = DateTime.UtcNow
            };

            await _vitalSignRepository.AddAsync(vitalSign);
            _logger.LogInformation("Successfully created vital sign with ID: {VitalSignId}", vitalSign.Id);

            // Return response DTO
            return new VitalSignResponseDto
            {
                Id = vitalSign.Id,
                PatientId = vitalSign.PatientId,
                RecordedByUserId = vitalSign.RecordedByUserId,
                Temperature = vitalSign.Temperature,
                BloodPressureSystolic = vitalSign.BloodPressureSystolic,
                BloodPressureDiastolic = vitalSign.BloodPressureDiastolic,
                HeartRate = vitalSign.HeartRate,
                OxygenSaturation = vitalSign.OxygenSaturation,
                RespiratoryRate = vitalSign.RespiratoryRate,
                Weight = vitalSign.Weight,
                Height = vitalSign.Height,
                Severity = vitalSign.Severity,
                Notes = vitalSign.Notes,
                RecordedAt = vitalSign.RecordedAt,
                PatientName = $"{patient.FirstName} {patient.LastName}",
                RecordedByUserName = $"{user.FirstName} {user.LastName}"
            };
        }

        public async Task<IEnumerable<VitalSignResponseDto>> GetPatientVitalSignsAsync(int patientId)
        {
            var vitalSigns = await _vitalSignRepository.GetByPatientIdAsync(patientId);
            
            return vitalSigns.Select(vs => new VitalSignResponseDto
            {
                Id = vs.Id,
                PatientId = vs.PatientId,
                RecordedByUserId = vs.RecordedByUserId,
                Temperature = vs.Temperature,
                BloodPressureSystolic = vs.BloodPressureSystolic,
                BloodPressureDiastolic = vs.BloodPressureDiastolic,
                HeartRate = vs.HeartRate,
                OxygenSaturation = vs.OxygenSaturation,
                RespiratoryRate = vs.RespiratoryRate,
                Weight = vs.Weight,
                Height = vs.Height,
                Severity = vs.Severity,
                Notes = vs.Notes,
                RecordedAt = vs.RecordedAt,
                PatientName = $"{vs.Patient.FirstName} {vs.Patient.LastName}",
                RecordedByUserName = $"{vs.RecordedByUser.FirstName} {vs.RecordedByUser.LastName}"
            });
        }

        public async Task<VitalSignResponseDto> GetVitalSignAsync(int id)
        {
            var vitalSign = await _vitalSignRepository.GetByIdAsync(id);
            if (vitalSign == null)
                throw new ArgumentException("Vital sign not found");

            return new VitalSignResponseDto
            {
                Id = vitalSign.Id,
                PatientId = vitalSign.PatientId,
                RecordedByUserId = vitalSign.RecordedByUserId,
                Temperature = vitalSign.Temperature,
                BloodPressureSystolic = vitalSign.BloodPressureSystolic,
                BloodPressureDiastolic = vitalSign.BloodPressureDiastolic,
                HeartRate = vitalSign.HeartRate,
                OxygenSaturation = vitalSign.OxygenSaturation,
                RespiratoryRate = vitalSign.RespiratoryRate,
                Weight = vitalSign.Weight,
                Height = vitalSign.Height,
                Severity = vitalSign.Severity,
                Notes = vitalSign.Notes,
                RecordedAt = vitalSign.RecordedAt,
                PatientName = $"{vitalSign.Patient.FirstName} {vitalSign.Patient.LastName}",
                RecordedByUserName = $"{vitalSign.RecordedByUser.FirstName} {vitalSign.RecordedByUser.LastName}"
            };
        }

        public async Task<IEnumerable<VitalSignResponseDto>> GetCriticalVitalSignsAsync()
        {
            var criticalVitalSigns = await _vitalSignRepository.GetCriticalVitalSignsAsync();
            
            return criticalVitalSigns.Select(vs => new VitalSignResponseDto
            {
                Id = vs.Id,
                PatientId = vs.PatientId,
                RecordedByUserId = vs.RecordedByUserId,
                Temperature = vs.Temperature,
                BloodPressureSystolic = vs.BloodPressureSystolic,
                BloodPressureDiastolic = vs.BloodPressureDiastolic,
                HeartRate = vs.HeartRate,
                OxygenSaturation = vs.OxygenSaturation,
                RespiratoryRate = vs.RespiratoryRate,
                Weight = vs.Weight,
                Height = vs.Height,
                Severity = vs.Severity,
                Notes = vs.Notes,
                RecordedAt = vs.RecordedAt,
                PatientName = $"{vs.Patient.FirstName} {vs.Patient.LastName}",
                RecordedByUserName = $"{vs.RecordedByUser.FirstName} {vs.RecordedByUser.LastName}"
            });
        }

        public VitalSignSeverity DetermineSeverity(CreateVitalSignDto vitalSign)
        {
            var severity = VitalSignSeverity.Normal;

            // Check temperature
            if (vitalSign.Temperature.HasValue)
            {
                if (vitalSign.Temperature >= 40.0m || vitalSign.Temperature <= 35.0m)
                    severity = VitalSignSeverity.Critical;
                else if (vitalSign.Temperature >= 39.0m || vitalSign.Temperature <= 36.0m)
                    severity = VitalSignSeverity.High;
                else if (vitalSign.Temperature >= 38.0m || vitalSign.Temperature <= 36.5m)
                    severity = VitalSignSeverity.Elevated;
            }

            // Check blood pressure
            if (vitalSign.BloodPressureSystolic.HasValue)
            {
                if (vitalSign.BloodPressureSystolic >= 180 || vitalSign.BloodPressureSystolic <= 90)
                    severity = VitalSignSeverity.Critical;
                else if (vitalSign.BloodPressureSystolic >= 160 || vitalSign.BloodPressureSystolic <= 100)
                    severity = VitalSignSeverity.High;
                else if (vitalSign.BloodPressureSystolic >= 140 || vitalSign.BloodPressureSystolic <= 110)
                    severity = VitalSignSeverity.Elevated;
            }

            // Check heart rate
            if (vitalSign.HeartRate.HasValue)
            {
                if (vitalSign.HeartRate >= 120 || vitalSign.HeartRate <= 50)
                    severity = VitalSignSeverity.Critical;
                else if (vitalSign.HeartRate >= 100 || vitalSign.HeartRate <= 60)
                    severity = VitalSignSeverity.High;
                else if (vitalSign.HeartRate >= 90 || vitalSign.HeartRate <= 70)
                    severity = VitalSignSeverity.Elevated;
            }

            // Check oxygen saturation
            if (vitalSign.OxygenSaturation.HasValue)
            {
                if (vitalSign.OxygenSaturation <= 90)
                    severity = VitalSignSeverity.Critical;
                else if (vitalSign.OxygenSaturation <= 95)
                    severity = VitalSignSeverity.High;
                else if (vitalSign.OxygenSaturation <= 97)
                    severity = VitalSignSeverity.Elevated;
            }

            // Check respiratory rate
            if (vitalSign.RespiratoryRate.HasValue)
            {
                if (vitalSign.RespiratoryRate >= 30 || vitalSign.RespiratoryRate <= 8)
                    severity = VitalSignSeverity.Critical;
                else if (vitalSign.RespiratoryRate >= 25 || vitalSign.RespiratoryRate <= 10)
                    severity = VitalSignSeverity.High;
                else if (vitalSign.RespiratoryRate >= 20 || vitalSign.RespiratoryRate <= 12)
                    severity = VitalSignSeverity.Elevated;
            }

            return severity;
        }
    }
} 
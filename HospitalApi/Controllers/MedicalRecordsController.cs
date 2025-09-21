using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalApi.Data;
using HospitalApi.Models;
using HospitalApi.Application.DTOs;
using HospitalApi.Infrastructure.Interfaces.Services;

namespace HospitalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly HospitalContext _context;
        private readonly INotificationService _notificationService;

        public MedicalRecordsController(HospitalContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // GET: api/medicalrecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalRecordResponseDto>>> GetMedicalRecords(
            [FromQuery] int? patientId = null,
            [FromQuery] int? doctorId = null,
            [FromQuery] bool? isCritical = null)
        {
            var query = _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .AsQueryable();

            if (patientId.HasValue)
                query = query.Where(mr => mr.PatientId == patientId.Value);

            if (doctorId.HasValue)
                query = query.Where(mr => mr.DoctorId == doctorId.Value);

            if (isCritical.HasValue)
                query = query.Where(mr => mr.IsCritical == isCritical.Value);

            var records = await query
                .Select(mr => new MedicalRecordResponseDto
                {
                    Id = mr.Id,
                    PatientId = mr.PatientId,
                    DoctorId = mr.DoctorId,
                    Diagnosis = mr.Diagnosis,
                    Symptoms = mr.Symptoms,
                    Treatment = mr.Treatment,
                    Prescriptions = mr.Prescriptions,
                    Temperature = mr.Temperature,
                    BloodPressureSystolic = mr.BloodPressureSystolic,
                    BloodPressureDiastolic = mr.BloodPressureDiastolic,
                    HeartRate = mr.HeartRate,
                    Weight = mr.Weight,
                    Height = mr.Height,
                    IsCritical = mr.IsCritical,
                    CriticalNotes = mr.CriticalNotes,
                    CreatedAt = mr.CreatedAt,
                    UpdatedAt = mr.UpdatedAt,
                    PatientName = $"{mr.Patient.FirstName} {mr.Patient.LastName}",
                    DoctorName = $"Dr. {mr.Doctor.LastName}"
                })
                .OrderByDescending(mr => mr.CreatedAt)
                .ToListAsync();

            return Ok(records);
        }

        // GET: api/medicalrecords/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecordResponseDto>> GetMedicalRecord(int id)
        {
            var record = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .Where(mr => mr.Id == id)
                .Select(mr => new MedicalRecordResponseDto
                {
                    Id = mr.Id,
                    PatientId = mr.PatientId,
                    DoctorId = mr.DoctorId,
                    Diagnosis = mr.Diagnosis,
                    Symptoms = mr.Symptoms,
                    Treatment = mr.Treatment,
                    Prescriptions = mr.Prescriptions,
                    Temperature = mr.Temperature,
                    BloodPressureSystolic = mr.BloodPressureSystolic,
                    BloodPressureDiastolic = mr.BloodPressureDiastolic,
                    HeartRate = mr.HeartRate,
                    Weight = mr.Weight,
                    Height = mr.Height,
                    IsCritical = mr.IsCritical,
                    CriticalNotes = mr.CriticalNotes,
                    CreatedAt = mr.CreatedAt,
                    UpdatedAt = mr.UpdatedAt,
                    PatientName = $"{mr.Patient.FirstName} {mr.Patient.LastName}",
                    DoctorName = $"Dr. {mr.Doctor.LastName}"
                })
                .FirstOrDefaultAsync();

            if (record == null)
                return NotFound();

            return Ok(record);
        }

        // POST: api/medicalrecords
        [HttpPost]
        public async Task<ActionResult<MedicalRecordResponseDto>> CreateMedicalRecord(CreateMedicalRecordDto dto)
        {
            // Verify patient and doctor exist
            var patient = await _context.Patients.FindAsync(dto.PatientId);
            var doctor = await _context.Users.FindAsync(dto.DoctorId);

            if (patient == null)
                return BadRequest("Patient not found");

            if (doctor == null || doctor.Role != UserRole.Doctor)
                return BadRequest("Doctor not found or invalid");

            var record = new MedicalRecord
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                Diagnosis = dto.Diagnosis,
                Symptoms = dto.Symptoms,
                Treatment = dto.Treatment,
                Prescriptions = dto.Prescriptions,
                Temperature = dto.Temperature,
                BloodPressureSystolic = dto.BloodPressureSystolic,
                BloodPressureDiastolic = dto.BloodPressureDiastolic,
                HeartRate = dto.HeartRate,
                Weight = dto.Weight,
                Height = dto.Height,
                IsCritical = dto.IsCritical,
                CriticalNotes = dto.CriticalNotes,
                CreatedAt = DateTime.UtcNow
            };

            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();

            // Update patient critical status if needed
            if (dto.IsCritical && !patient.IsCritical)
            {
                patient.IsCritical = true;
                patient.LastUpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await _notificationService.CreateCriticalPatientAlertAsync(
                    patient.Id,
                    $"Patient {patient.FirstName} {patient.LastName} has critical medical condition: {dto.Diagnosis}"
                );
            }

            // Create notification for medical record update
            await _notificationService.CreateMedicalRecordUpdateNotificationAsync(
                dto.PatientId, dto.DoctorId, dto.Diagnosis);

            var response = new MedicalRecordResponseDto
            {
                Id = record.Id,
                PatientId = record.PatientId,
                DoctorId = record.DoctorId,
                Diagnosis = record.Diagnosis,
                Symptoms = record.Symptoms,
                Treatment = record.Treatment,
                Prescriptions = record.Prescriptions,
                Temperature = record.Temperature,
                BloodPressureSystolic = record.BloodPressureSystolic,
                BloodPressureDiastolic = record.BloodPressureDiastolic,
                HeartRate = record.HeartRate,
                Weight = record.Weight,
                Height = record.Height,
                IsCritical = record.IsCritical,
                CriticalNotes = record.CriticalNotes,
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt,
                PatientName = $"{patient.FirstName} {patient.LastName}",
                DoctorName = $"Dr. {doctor.LastName}"
            };

            return CreatedAtAction(nameof(GetMedicalRecord), new { id = record.Id }, response);
        }

        // PUT: api/medicalrecords/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicalRecord(int id, UpdateMedicalRecordDto dto)
        {
            var record = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .FirstOrDefaultAsync(mr => mr.Id == id);

            if (record == null)
                return NotFound();

            bool wasCritical = record.IsCritical;

            if (dto.Diagnosis != null)
                record.Diagnosis = dto.Diagnosis;
            if (dto.Symptoms != null)
                record.Symptoms = dto.Symptoms;
            if (dto.Treatment != null)
                record.Treatment = dto.Treatment;
            if (dto.Prescriptions != null)
                record.Prescriptions = dto.Prescriptions;
            if (dto.Temperature.HasValue)
                record.Temperature = dto.Temperature.Value;
            if (dto.BloodPressureSystolic.HasValue)
                record.BloodPressureSystolic = dto.BloodPressureSystolic.Value;
            if (dto.BloodPressureDiastolic.HasValue)
                record.BloodPressureDiastolic = dto.BloodPressureDiastolic.Value;
            if (dto.HeartRate.HasValue)
                record.HeartRate = dto.HeartRate.Value;
            if (dto.Weight.HasValue)
                record.Weight = dto.Weight.Value;
            if (dto.Height.HasValue)
                record.Height = dto.Height.Value;
            if (dto.IsCritical.HasValue)
                record.IsCritical = dto.IsCritical.Value;
            if (dto.CriticalNotes != null)
                record.CriticalNotes = dto.CriticalNotes;

            record.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Handle critical status changes
            if (!wasCritical && record.IsCritical)
            {
                record.Patient.IsCritical = true;
                record.Patient.LastUpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await _notificationService.CreateCriticalPatientAlertAsync(
                    record.PatientId,
                    $"Patient {record.Patient.FirstName} {record.Patient.LastName} has critical medical condition: {record.Diagnosis}"
                );
            }
            else if (wasCritical && !record.IsCritical)
            {
                record.Patient.IsCritical = false;
                record.Patient.LastUpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                {
                    Title = "Patient Status Update",
                    Message = $"Patient {record.Patient.FirstName} {record.Patient.LastName} medical condition is now stable.",
                    Type = NotificationType.PatientUpdate,
                    Priority = NotificationPriority.Normal,
                    PatientId = record.PatientId
                });
            }

            return NoContent();
        }

        // DELETE: api/medicalrecords/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            var record = await _context.MedicalRecords.FindAsync(id);
            if (record == null)
                return NotFound();

            _context.MedicalRecords.Remove(record);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/medicalrecords/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<MedicalRecordResponseDto>>> GetPatientMedicalRecords(int patientId)
        {
            var records = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .Where(mr => mr.PatientId == patientId)
                .Select(mr => new MedicalRecordResponseDto
                {
                    Id = mr.Id,
                    PatientId = mr.PatientId,
                    DoctorId = mr.DoctorId,
                    Diagnosis = mr.Diagnosis,
                    Symptoms = mr.Symptoms,
                    Treatment = mr.Treatment,
                    Prescriptions = mr.Prescriptions,
                    Temperature = mr.Temperature,
                    BloodPressureSystolic = mr.BloodPressureSystolic,
                    BloodPressureDiastolic = mr.BloodPressureDiastolic,
                    HeartRate = mr.HeartRate,
                    Weight = mr.Weight,
                    Height = mr.Height,
                    IsCritical = mr.IsCritical,
                    CriticalNotes = mr.CriticalNotes,
                    CreatedAt = mr.CreatedAt,
                    UpdatedAt = mr.UpdatedAt,
                    PatientName = $"{mr.Patient.FirstName} {mr.Patient.LastName}",
                    DoctorName = $"Dr. {mr.Doctor.LastName}"
                })
                .OrderByDescending(mr => mr.CreatedAt)
                .ToListAsync();

            return Ok(records);
        }

        // GET: api/medicalrecords/critical
        [HttpGet("critical")]
        public async Task<ActionResult<IEnumerable<MedicalRecordResponseDto>>> GetCriticalMedicalRecords()
        {
            var criticalRecords = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .Where(mr => mr.IsCritical)
                .Select(mr => new MedicalRecordResponseDto
                {
                    Id = mr.Id,
                    PatientId = mr.PatientId,
                    DoctorId = mr.DoctorId,
                    Diagnosis = mr.Diagnosis,
                    Symptoms = mr.Symptoms,
                    Treatment = mr.Treatment,
                    Prescriptions = mr.Prescriptions,
                    Temperature = mr.Temperature,
                    BloodPressureSystolic = mr.BloodPressureSystolic,
                    BloodPressureDiastolic = mr.BloodPressureDiastolic,
                    HeartRate = mr.HeartRate,
                    Weight = mr.Weight,
                    Height = mr.Height,
                    IsCritical = mr.IsCritical,
                    CriticalNotes = mr.CriticalNotes,
                    CreatedAt = mr.CreatedAt,
                    UpdatedAt = mr.UpdatedAt,
                    PatientName = $"{mr.Patient.FirstName} {mr.Patient.LastName}",
                    DoctorName = $"Dr. {mr.Doctor.LastName}"
                })
                .OrderByDescending(mr => mr.CreatedAt)
                .ToListAsync();

            return Ok(criticalRecords);
        }
    }
} 
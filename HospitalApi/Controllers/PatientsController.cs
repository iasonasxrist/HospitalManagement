using Microsoft.AspNetCore.Mvc;
using HospitalApi.Models;
using HospitalApi.Application.DTOs;
using HospitalApi.Infrastructure.Interfaces.Services;

namespace HospitalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // GET: api/patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetPatients(
            [FromQuery] bool? isCritical = null,
            [FromQuery] PatientStatus? status = null,
            [FromQuery] string? search = null)
        {
            var patients = await _patientService.GetPatientsAsync(isCritical, status, search);
            return Ok(patients);
        }

        // GET: api/patients/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientResponseDto>> GetPatient(int id)
        {
            var patient = await _patientService.GetPatientAsync(id);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        // POST: api/patients
        [HttpPost]
        public async Task<ActionResult<PatientResponseDto>> CreatePatient(CreatePatientDto dto)
        {
            var patient = await _patientService.CreatePatientAsync(dto);
            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        // PUT: api/patients/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, UpdatePatientDto dto)
        {
            var success = await _patientService.UpdatePatientAsync(id, dto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/patients/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var success = await _patientService.DeletePatientAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // GET: api/patients/critical
        [HttpGet("critical")]
        public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetCriticalPatients()
        {
            var criticalPatients = await _patientService.GetCriticalPatientsAsync();
            return Ok(criticalPatients);
        }

        // POST: api/patients/{id}/mark-critical
        [HttpPost("{id}/mark-critical")]
        public async Task<IActionResult> MarkPatientCritical(int id, [FromBody] string reason)
        {
            var success = await _patientService.MarkPatientCriticalAsync(id, reason);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // POST: api/patients/{id}/mark-stable
        [HttpPost("{id}/mark-stable")]
        public async Task<IActionResult> MarkPatientStable(int id)
        {
            var success = await _patientService.MarkPatientStableAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
} 
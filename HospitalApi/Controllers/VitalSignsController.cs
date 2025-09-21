using Microsoft.AspNetCore.Mvc;
using HospitalApi.Infrastructure.Interfaces.Services;
using HospitalApi.Application.DTOs;
using HospitalApi.Authorization;
using HospitalApi.Models;

namespace HospitalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VitalSignsController : ControllerBase
    {
        private readonly IVitalSignService _vitalSignService;

        public VitalSignsController(IVitalSignService vitalSignService)
        {
            _vitalSignService = vitalSignService;
        }

        // GET: api/vitalsigns
        [HttpGet]
        [AuthorizeRoles(UserRole.Doctor, UserRole.Nurse, UserRole.Admin)]
        public async Task<ActionResult<IEnumerable<VitalSignResponseDto>>> GetVitalSigns()
        {
            var vitalSigns = await _vitalSignService.GetCriticalVitalSignsAsync();
            return Ok(vitalSigns);
        }

        // GET: api/vitalsigns/{id}
        [HttpGet("{id}")]
        [AuthorizeRoles(UserRole.Doctor, UserRole.Nurse, UserRole.Admin)]
        public async Task<ActionResult<VitalSignResponseDto>> GetVitalSign(int id)
        {
            try
            {
                var vitalSign = await _vitalSignService.GetVitalSignAsync(id);
                return Ok(vitalSign);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        // POST: api/vitalsigns
        [HttpPost]
        [AuthorizeRoles(UserRole.Doctor, UserRole.Nurse)]
        public async Task<ActionResult<VitalSignResponseDto>> CreateVitalSign(CreateVitalSignDto dto)
        {
            try
            {
                var vitalSign = await _vitalSignService.CreateVitalSignAsync(dto);
                return CreatedAtAction(nameof(GetVitalSign), new { id = vitalSign.Id }, vitalSign);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/vitalsigns/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        [AuthorizeRoles(UserRole.Doctor, UserRole.Nurse, UserRole.Admin)]
        public async Task<ActionResult<IEnumerable<VitalSignResponseDto>>> GetPatientVitalSigns(int patientId)
        {
            var vitalSigns = await _vitalSignService.GetPatientVitalSignsAsync(patientId);
            return Ok(vitalSigns);
        }

        // GET: api/vitalsigns/critical
        [HttpGet("critical")]
        [AuthorizeRoles(UserRole.Doctor, UserRole.Nurse, UserRole.Admin)]
        public async Task<ActionResult<IEnumerable<VitalSignResponseDto>>> GetCriticalVitalSigns()
        {
            var criticalVitalSigns = await _vitalSignService.GetCriticalVitalSignsAsync();
            return Ok(criticalVitalSigns);
        }

        // GET: api/vitalsigns/latest/{patientId}
        [HttpGet("latest/{patientId}")]
        [AuthorizeRoles(UserRole.Doctor, UserRole.Nurse, UserRole.Admin)]
        public async Task<ActionResult<VitalSignResponseDto>> GetLatestVitalSigns(int patientId)
        {
            var vitalSigns = await _vitalSignService.GetPatientVitalSignsAsync(patientId);
            var latest = vitalSigns.FirstOrDefault();
            
            if (latest == null)
                return NotFound();

            return Ok(latest);
        }
    }
} 
using HospitalApi.Application.DTOs;
using HospitalApi.Models;

namespace HospitalApi.Infrastructure.Interfaces.Services
{
    public interface IVitalSignService
    {
        Task<VitalSignResponseDto> CreateVitalSignAsync(CreateVitalSignDto dto);
        Task<IEnumerable<VitalSignResponseDto>> GetPatientVitalSignsAsync(int patientId);
        Task<VitalSignResponseDto> GetVitalSignAsync(int id);
        Task<IEnumerable<VitalSignResponseDto>> GetCriticalVitalSignsAsync();
        VitalSignSeverity DetermineSeverity(CreateVitalSignDto vitalSign);
    }
} 
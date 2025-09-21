using HospitalApi.Application.DTOs;
using HospitalApi.Models;

namespace HospitalApi.Infrastructure.Interfaces.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientResponseDto>> GetPatientsAsync(bool? isCritical = null, PatientStatus? status = null, string? search = null);
        Task<PatientResponseDto?> GetPatientAsync(int id);
        Task<PatientResponseDto> CreatePatientAsync(CreatePatientDto dto);
        Task<bool> UpdatePatientAsync(int id, UpdatePatientDto dto);
        Task<bool> DeletePatientAsync(int id);
        Task<IEnumerable<PatientResponseDto>> GetCriticalPatientsAsync();
        Task<bool> MarkPatientCriticalAsync(int id, string reason);
        Task<bool> MarkPatientStableAsync(int id);
    }
} 
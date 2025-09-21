using HospitalApi.Models;

namespace HospitalApi.Infrastructure.Interfaces.Repositories
{
    public interface IVitalSignRepository : IGenericRepository<VitalSign>
    {
        Task<IEnumerable<VitalSign>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<VitalSign>> GetCriticalVitalSignsAsync();
        Task<VitalSign?> GetLatestByPatientIdAsync(int patientId);
    }
} 
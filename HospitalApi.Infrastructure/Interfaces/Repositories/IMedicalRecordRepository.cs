using HospitalApi.Models;

namespace HospitalApi.Infrastructure.Interfaces.Repositories
{
    public interface IMedicalRecordRepository : IGenericRepository<MedicalRecord>
    {
        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<MedicalRecord>> GetCriticalRecordsAsync();
    }
} 
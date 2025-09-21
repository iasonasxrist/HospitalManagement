using HospitalApi.Models;

namespace HospitalApi.Infrastructure.Interfaces.Repositories
{
    public interface IPatientRepository : IGenericRepository<Patient>
    {
        Task<IEnumerable<Patient>> GetCriticalPatientsAsync();
        Task<IEnumerable<Patient>> GetByStatusAsync(PatientStatus status);
        Task<IEnumerable<Patient>> SearchAsync(string searchTerm);
    }
} 
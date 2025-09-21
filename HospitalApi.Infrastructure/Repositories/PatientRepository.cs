using HospitalApi.Infrastructure.Interfaces.Repositories;
using HospitalApi.Data;
using HospitalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Infrastructure.Repositories
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public PatientRepository(HospitalContext context) : base(context) { }

        public async Task<IEnumerable<Patient>> GetByStatusAsync(PatientStatus status)
        {
            return await _dbSet.Where(p => p.Status == status).ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetCriticalPatientsAsync()
        {
            return await _dbSet.Where(p => p.IsCritical).ToListAsync();
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string searchTerm)
        {
            return await _dbSet.Where(p => 
                p.FirstName.Contains(searchTerm) || 
                p.LastName.Contains(searchTerm) ||
                p.PhoneNumber.Contains(searchTerm)).ToListAsync();
        }


    }
} 
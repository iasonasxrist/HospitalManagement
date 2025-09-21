using HospitalApi.Infrastructure.Interfaces.Repositories;
using HospitalApi.Data;
using HospitalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Infrastructure.Repositories
{
    public class VitalSignRepository : GenericRepository<VitalSign>, IVitalSignRepository
    {
        public VitalSignRepository(HospitalContext context) : base(context) { }

        public async Task<IEnumerable<VitalSign>> GetByPatientIdAsync(int patientId)
        {
            return await _dbSet
                .Include(vs => vs.Patient)
                .Include(vs => vs.RecordedByUser)
                .Where(vs => vs.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<VitalSign>> GetCriticalVitalSignsAsync()
        {
            return await _dbSet
                .Include(vs => vs.Patient)
                .Include(vs => vs.RecordedByUser)
                .Where(vs => vs.Severity == VitalSignSeverity.Critical)
                .ToListAsync();
        }

        public async Task<VitalSign?> GetLatestByPatientIdAsync(int patientId)
        {
            return await _dbSet
                .Include(vs => vs.Patient)
                .Include(vs => vs.RecordedByUser)
                .Where(vs => vs.PatientId == patientId)
                .OrderByDescending(vs => vs.RecordedAt)
                .FirstOrDefaultAsync();
        }
    }
} 
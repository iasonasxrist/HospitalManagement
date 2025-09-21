using HospitalApi.Infrastructure.Interfaces.Repositories;
using HospitalApi.Data;
using HospitalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Infrastructure.Repositories
{
    public class MedicalRecordRepository : GenericRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(HospitalContext context) : base(context) { }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId)
        {
            return await _dbSet.Where(mr => mr.PatientId == patientId).ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(int doctorId)
        {
            return await _dbSet.Where(mr => mr.DoctorId == doctorId).ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetCriticalRecordsAsync()
        {
            return await _dbSet.Where(mr => mr.IsCritical).ToListAsync();
        }
    }
} 
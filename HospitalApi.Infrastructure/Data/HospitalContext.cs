using Microsoft.EntityFrameworkCore;
using HospitalApi.Models;

namespace HospitalApi.Data
{
    public class HospitalContext : DbContext
    {
        public HospitalContext(DbContextOptions<HospitalContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<VitalSign> VitalSigns { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<LabResult> LabResults { get; set; }
        public DbSet<ProgressNote> ProgressNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasConversion<string>();
            });

            // Patient configuration
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasIndex(e => new { e.FirstName, e.LastName, e.DateOfBirth });
                entity.HasIndex(e => e.Room);
                entity.HasIndex(e => e.Department);
                entity.Property(e => e.Status).HasConversion<string>();
            });

            // MedicalRecord configuration
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.MedicalRecords)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Doctor)
                    .WithMany()
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Appointment configuration
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Doctor)
                    .WithMany()
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Status).HasConversion<string>();
            });

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(e => e.Type).HasConversion<string>();
                entity.Property(e => e.Priority).HasConversion<string>();
                entity.Property(e => e.Severity).HasConversion<string>();
            });

            // VitalSign configuration
            modelBuilder.Entity<VitalSign>(entity =>
            {
                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.VitalSigns)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.RecordedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.RecordedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Severity).HasConversion<string>();
            });

            // Prescription configuration
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.PrescribedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.PrescribedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Status).HasConversion<string>();
            });

            // LabResult configuration
            modelBuilder.Entity<LabResult>(entity =>
            {
                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.LabResults)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.OrderedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.OrderedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.Severity).HasConversion<string>();
            });

            // ProgressNote configuration
            modelBuilder.Entity<ProgressNote>(entity =>
            {
                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.ProgressNotes)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Type).HasConversion<string>();
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed admin user
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@hospital.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FirstName = "System",
                LastName = "Administrator",
                Role = UserRole.Admin,
                PhoneNumber = "+1234567890",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });

            // Seed doctor
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 2,
                Username = "dr.smith",
                Email = "dr.smith@hospital.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("doctor123"),
                FirstName = "John",
                LastName = "Smith",
                Role = UserRole.Doctor,
                PhoneNumber = "+1234567891",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });

            // Seed nurse
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 3,
                Username = "nurse.jones",
                Email = "nurse.jones@hospital.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("nurse123"),
                FirstName = "Sarah",
                LastName = "Jones",
                Role = UserRole.Nurse,
                PhoneNumber = "+1234567892",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });

            // Seed sample patient - Sarah Johnson
            modelBuilder.Entity<Patient>().HasData(new Patient
            {
                Id = 1,
                FirstName = "Sarah",
                LastName = "Johnson",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = "Female",
                Address = "123 Main St, City, ST 12345",
                PhoneNumber = "(555) 123-4567",
                Email = "patient@email.com",
                EmergencyContact = "John Doe (Spouse)",
                EmergencyPhone = "(555) 987-6543",
                MedicalHistory = "Post-Surgery",
                Allergies = "Penicillin, Latex",
                BloodType = "O+",
                Room = "A-101",
                Department = "Cardiology",
                Condition = "Post-Surgery",
                Status = PatientStatus.Active,
                IsCritical = false,
                CreatedAt = DateTime.UtcNow
            });

            // Seed sample patient - Michael Chen (Critical)
            modelBuilder.Entity<Patient>().HasData(new Patient
            {
                Id = 2,
                FirstName = "Michael",
                LastName = "Chen",
                DateOfBirth = new DateTime(1975, 8, 22),
                Gender = "Male",
                Address = "456 Oak Ave, City, ST 12345",
                PhoneNumber = "(555) 234-5678",
                Email = "michael.chen@email.com",
                EmergencyContact = "Lisa Chen (Spouse)",
                EmergencyPhone = "(555) 876-5432",
                MedicalHistory = "Hypertension, Diabetes",
                Allergies = "Sulfa drugs",
                BloodType = "A+",
                Room = "B-205",
                Department = "Cardiology",
                Condition = "Hypertensive Crisis",
                Status = PatientStatus.Active,
                IsCritical = true,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
} 
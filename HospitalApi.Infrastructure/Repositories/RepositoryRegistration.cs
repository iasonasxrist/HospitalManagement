using HospitalApi.Infrastructure.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalApi.Infrastructure.Repositories
{
    public static class RepositoryRegistration
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<IVitalSignRepository, VitalSignRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            return services;
        }
    }
} 
using HospitalApi.Models;

namespace HospitalApi.Infrastructure.Interfaces.Repositories
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Notification>> GetUnreadAsync(int? userId = null);
    }
} 
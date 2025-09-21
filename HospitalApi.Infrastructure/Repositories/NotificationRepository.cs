using HospitalApi.Infrastructure.Interfaces.Repositories;
using HospitalApi.Data;
using HospitalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(HospitalContext context) : base(context) { }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
        {
            return await _dbSet.Where(n => n.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadAsync(int? userId = null)
        {
            var query = _dbSet.Where(n => !n.IsRead);
            if (userId.HasValue)
                query = query.Where(n => n.UserId == userId.Value);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetCriticalNotificationsAsync()
        {
            return await _dbSet.Where(n => n.Priority == NotificationPriority.Critical).ToListAsync();
        }
    }
} 
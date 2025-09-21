using HospitalApi.Models;

namespace HospitalApi.Infrastructure.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
    }
} 
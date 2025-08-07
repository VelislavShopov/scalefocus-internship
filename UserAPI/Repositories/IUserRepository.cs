using UserAPI.DTOs;
using UserAPI.Models;

namespace UserAPI.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
        Task CreateUser(User user);

        Task DeleteUser(Guid id);

        Task<User> GetUser(Guid id);

        Task<User> GetUserByUsername(string username);

        Task<User> GetUserByEmail(string email);

        Task<List<UserRole>> GetRolesForUser(User user);

        Task<User?> GetUserByResetToken(string token);
      
        Task UpdateUser(User user);
    }
}

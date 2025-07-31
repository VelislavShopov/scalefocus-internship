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

        void GetRolesForUser(User user);

    }
}

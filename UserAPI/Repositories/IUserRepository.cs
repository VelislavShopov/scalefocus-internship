using UserAPI.Models;

namespace UserAPI.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
        Task CreateUser(User user);

        Task DeleteUser(int id);

        Task<User> GetUser(int id);
    }
}

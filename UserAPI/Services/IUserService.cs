using UserAPI.Models;

namespace UserAPI.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task CreateUser(User user);

        Task<User> GetUser(int id);
        Task DeleteUser(int id);
    }
}


using UserAPI.Models;
using UserAPI.DTOs;

namespace UserAPI.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task CreateUser(UserDTO user);
        
        Task<User> GetUser(int id);
        Task DeleteUser(int id);

        //Login task в интерфейса, добавен от Георги Станков.
        Task<string> LoginAsync(UserDTO request)

    }
}
